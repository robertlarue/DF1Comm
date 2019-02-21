'**********************************************************************************************
'* DF1 Data Link Layer & Application Layer
'*
'* Archie Jacobs
'* Manufacturing Automation, LLC
'* ajacobs@mfgcontrol.com
'* 22-NOV-06
'*
'* This class implements the two layers of the Allen Bradley DF1 protocol.
'* In terms of the AB documentation, the data link layer acts as the transmitter and receiver.
'* Communication commands in the format described in chapter 7, are passed to
'* the data link layer using the SendData method.
'*
'* Reference : Allen Bradley Publication 1770-6.5.16
'*
'* Distributed under the GNU General Public License (www.gnu.org)
'*
'* This program is free software; you can redistribute it and/or
'* as published by the Free Software Foundation; either version 2
'* of the License, or (at your option) any later version.
'*
'* This program is distributed in the hope that it will be useful,
'* but WITHOUT ANY WARRANTY; without even the implied warranty of
'* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'* GNU General Public License for more details.

'* You should have received a copy of the GNU General Public License
'* along with this program; if not, write to the Free Software
'* Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
'*
'*
'* 22-MAR-07  Added floating point read/write capability
'* 23-MAR-07  Added string file read/write
'*              Handle reading of up to 256 elements in one call
'* 24-MAR-07  Corrected ReadAny to allow an array of strings to be read
'* 26-MAR-07  Changed error handling to throw exceptions to comply more with .NET standards
'* 29-MAR-07  When reading multiple sub elements of timers or counters, read all the same sub-element
'* 30-MAR-07  Added GetDataMemory, GetSlotCount, and GetIoConfig  - all were reverse engineered
'* 04-APR-07  Added GetMicroDataMemory
'* 07-APR-07  Reworked the Responded variable to try to fix a small issue during a lot of rapid reads
'* 12-APR-07  Fixed a problem with reading Timers & Counters  more than 39 at a time
'* 01-MAY-07  Fixed a problem with writing multiple elements using WriteData
'* 06-JUN-07  Add the assumption of file number 2 for S file (ParseAddress) e.g. S:1=S2:1
'* 30-AUG-07  Fixed a problem where the value 16 gets doubled, it would not check the last byte
'* 13-FEB-08  Added more errors codes in DecodeMessage, Added the EXT STS if STS=&hF0
'* 13-FEB-08  Added GetML1500DataMemory to work with the MicroLogix 1500
'* 14-FEB-08  Added Reading/Writing of Long data with help from Tony Cicchino
'* 14-FEB-08  Corrected problem when writing an array of Singles to an Integer table
'* 18-FEB-08  Corrected an error in SendData that would not allow it to retry
'* 23-FEB-08  Corrected a problem when reading elements with extended addressing
'* 26-FEB-08  Reconfigured ReadRawData & WriteRawData
'* 28-FEB-08  Completed Downloading & Uploading functions
'**********************************************************************************************
Imports System.ComponentModel.Design
Imports System.IO.Ports
Imports System.Text.RegularExpressions
'<Assembly: system.Security.Permissions.SecurityPermissionAttribute(system.Security.Permissions.SecurityAction.Demand)> 
<Assembly: CLSCompliant(True)>
Public Class DF1Comm
    '* create a random number as a TNS starting point
    Private rnd As New Random
    Private TNS As UInt16 = (rnd.Next And &H7F) + 1
    Private ProcessorType As Integer
    '* This is used to help problems that come from transmissions errors when using a USB converter
    Private SleepDelay As Integer

    Private Responded(255) As Boolean
    Private QueuedCommand As New System.Collections.ObjectModel.Collection(Of Byte)
    Private CommandInQueue As Boolean

    Private DisableEvent As Boolean

    Public Event DataReceived As EventHandler
    Public Event UnsolictedMessageRcvd As EventHandler
    Public Event AutoDetectTry As EventHandler
    Public Event DownloadProgress As EventHandler
    Public Event UploadProgress As EventHandler


#Region "Properties"
    Private m_MyNode As Integer
    Public Property MyNode() As Integer
        Get
            Return m_MyNode
        End Get
        Set(ByVal value As Integer)
            m_MyNode = value
        End Set
    End Property

    Private m_TargetNode As Integer
    Public Property TargetNode() As Integer
        Get
            Return m_TargetNode
        End Get
        Set(ByVal value As Integer)
            m_TargetNode = value
        End Set
    End Property

    Private m_BaudRate As Integer = 19200
    Public Property BaudRate() As Integer
        Get
            Return m_BaudRate
        End Get
        Set(ByVal value As Integer)
            If value <> m_BaudRate Then CloseComms()
            m_BaudRate = value
        End Set
    End Property

    Private m_ComPort As String = "COM1"
    Public Property ComPort() As String
        Get
            Return m_ComPort
        End Get
        Set(ByVal value As String)
            If value <> m_ComPort Then CloseComms()
            m_ComPort = value
        End Set
    End Property

    Private m_Parity As System.IO.Ports.Parity = IO.Ports.Parity.None
    Public Property Parity() As System.IO.Ports.Parity
        Get
            Return m_Parity
        End Get
        Set(ByVal value As System.IO.Ports.Parity)
            If value <> m_Parity Then CloseComms()
            m_Parity = value
        End Set
    End Property

    Private m_Protocol As String = "DF1"
    Public Property Protocol() As String
        Get
            Return m_Protocol
        End Get
        Set(ByVal value As String)
            m_Protocol = value
        End Set
    End Property

    Public Enum CheckSumOptions
        Crc = 0
        Bcc = 1
    End Enum

    Private m_CheckSum As CheckSumOptions
    Public Property CheckSum() As CheckSumOptions
        Get
            Return m_CheckSum
        End Get
        Set(ByVal value As CheckSumOptions)
            m_CheckSum = value
        End Set
    End Property
    '**************************************************************
    '* Determine whether to wait for a data read or raise an event
    '**************************************************************
    Private m_AsyncMode As Boolean
    Public Property AsyncMode() As Boolean
        Get
            Return m_AsyncMode
        End Get
        Set(ByVal value As Boolean)
            m_AsyncMode = value
        End Set
    End Property

#End Region

#Region "Public Methods"
    '***************************************
    '* COMMAND IMPLEMENTATION SECTION
    '***************************************
    Public Sub SetRunMode()
        '* Get the processor type by using a get status command
        Dim reply As Integer
        Dim rTNS As Integer
        Dim data(0) As Byte
        Dim Func As Integer

        If GetProcessorType() = &H58 Then  '* ML1000
            data(0) = 2
            Func = &H3A
        Else
            Func = &H80
            data(0) = 6
        End If

        reply = PrefixAndSend(&HF, Func, data, True, rTNS)

        If reply <> 0 Then Throw New DF1Exception("Failed to change to Run mode, Check PLC Key switch - " & DecodeMessage(reply))
    End Sub

    Public Sub SetProgramMode()
        '* Get the processor type by using a get status command
        Dim reply As Integer
        Dim rTNS As Integer
        Dim data(0) As Byte
        Dim Func As Integer

        If GetProcessorType() = &H58 Then '* ML1000
            data(0) = 0
            Func = &H3A
        Else
            data(0) = 1
            Func = &H80
        End If

        reply = PrefixAndSend(&HF, Func, data, True, rTNS)

        If reply <> 0 Then Throw New DF1Exception("Failed to change to Program mode, Check PLC Key switch - " & DecodeMessage(reply))
    End Sub


    'Public Sub DisableForces(ByVal targetNode As Integer)
    '    Dim rTNS As Integer
    '    Dim data() As Byte = {}
    '    Dim reply As Integer = PrefixAndSend(TargetNode, &HF, &H41, data, True, rTNS)
    'End Sub

    ''' <summary>
    ''' Retreives the processor code by using the get status command
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetProcessorType() As Integer
        '* Get the processor type by using a get status command
        Dim rTNS As Integer
        Dim Data(0) As Byte
        If PrefixAndSend(6, 3, Data, True, rTNS) = 0 Then
            '* Returned data psoition 11 is the first character in the ASCII name of the processor
            '* Position 9 is the code for the processor
            '* &H78 = SLC 5/05
            '* &h89 = ML1500 LSP
            '* &H8C = ML1500 LRP
            '* &H1A = Fixed SLC500
            '* &H18 = SLC 5/01
            '* &H25 = SLC 5/02
            '* &H49 = SLC 5/03
            '* &H5B = SLC 5/04
            '* &H95 = CompactLogix L35E
            '* &H58 = ML1000
            '* &H9C = ML1100
            '* &H88 = ML1200
            ProcessorType = DataPackets(rTNS)(9)
        End If
        Return DataPackets(rTNS)(9)
    End Function


    Public Structure DataFileDetails
        Dim FileType As String
        Dim FileNumber As Integer
        Dim NumberOfElements As Integer
    End Structure

    ''' <summary>
    ''' Retreives the list of data tables and number of elements in each
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataMemoryX() As DataFileDetails()
        'Dim Data(0) As Byte
        Dim ProcessorType As Integer = GetProcessorType()

        '* See GetProcessorType for codes
        Select Case ProcessorType
            'Case &H89
            '    Return GetML1500DataMemory()
            'Case &H58, &H25
            '    Return GetML1000DataMemory()
            Case Else
                'Return GetSLCDataMemory()
        End Select

        Throw New DF1Exception("Could Not Get processor type")
    End Function


    '*******************************************************************
    '* This is the start of reverse engineering to retreive data tables
    '*   Read 12 bytes File #0, Type 1, start at Element 21
    '*    Then extract the number of data and program files
    '*******************************************************************
    ''' <summary>
    ''' Retreives the list of data tables and number of elements in each
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetDataMemory() As DataFileDetails()
        '**************************************************
        '* Read the File 0 (Program & data file directory
        '**************************************************
        Dim FileZeroData() As Byte = ReadFileDirectory()


        Dim NumberOfDataTables As Integer = FileZeroData(52) + FileZeroData(53) * 256
        Dim NumberOfProgramFiles As Integer = FileZeroData(46) + FileZeroData(47) * 256
        'Dim DataFiles(NumberOfDataTables - 1) As DataFileDetails
        Dim DataFiles As New System.Collections.ObjectModel.Collection(Of DataFileDetails)
        Dim FilePosition As Integer
        Dim BytesPerRow As Integer
        '*****************************************
        '* Process the data from the data table
        '*****************************************
        Select Case ProcessorType
            Case &H25, &H58 '*ML1000, SLC 5/02
                FilePosition = 93
                BytesPerRow = 8
            Case &H88, &H89, &H8C, &H9C   '* ML1100, ML1200, ML1500
                FilePosition = 103
                BytesPerRow = 10
            Case Else               '* SLC 5/04, 5/05
                FilePosition = 79
                BytesPerRow = 10
        End Select


        '* Comb through data file 0 looking for data table definitions
        Dim i, k, BytesPerElement As Integer
        i = 0

        Dim DataFile As New DataFileDetails
        While k < NumberOfDataTables And FilePosition < FileZeroData.Length
            Select Case FileZeroData(FilePosition)
                Case &H82, &H8B : DataFile.FileType = "O"
                    BytesPerElement = 2
                Case &H83, &H8C : DataFile.FileType = "I"
                    BytesPerElement = 2
                Case &H84 : DataFile.FileType = "S"
                    BytesPerElement = 2
                Case &H85 : DataFile.FileType = "B"
                    BytesPerElement = 2
                Case &H86 : DataFile.FileType = "T"
                    BytesPerElement = 6
                Case &H87 : DataFile.FileType = "C"
                    BytesPerElement = 6
                Case &H88 : DataFile.FileType = "R"
                    BytesPerElement = 6
                Case &H89 : DataFile.FileType = "N"
                    BytesPerElement = 2
                Case &H8A : DataFile.FileType = "F"
                    BytesPerElement = 4
                Case &H8D : DataFile.FileType = "ST"
                    BytesPerElement = 84
                Case &H8E : DataFile.FileType = "A"
                    BytesPerElement = 2
                Case &H91 : DataFile.FileType = "L"   'Long Integer
                    BytesPerElement = 4
                Case &H92 : DataFile.FileType = "MG"   'Message Command 146
                    BytesPerElement = 50
                Case &H93 : DataFile.FileType = "PD"   'PID
                    BytesPerElement = 46
                Case &H94 : DataFile.FileType = "PLS"   'Programmable Limit Swith
                    BytesPerElement = 12

                Case Else : DataFile.FileType = "Undefined" '* 61h=Program File
                    BytesPerElement = 2
            End Select
            DataFile.NumberOfElements = (FileZeroData(FilePosition + 1) + FileZeroData(FilePosition + 2) * 256) / BytesPerElement
            DataFile.FileNumber = i

            '* Only return valid user data files
            If FileZeroData(FilePosition) > &H81 And FileZeroData(FilePosition) < &H9F Then
                DataFiles.Add(DataFile)
                'DataFile = New DataFileDetails
                k += 1
            End If

            '* Index file number once in the region of data files
            If k > 0 Then i += 1
            FilePosition += BytesPerRow
        End While

        '* Move to an array with a length of only good data files
        'Dim GoodDataFiles(k - 1) As DataFileDetails
        Dim GoodDataFiles(DataFiles.Count - 1) As DataFileDetails
        'For l As Integer = 0 To k - 1
        '    GoodDataFiles(l) = DataFiles(l)
        'Next

        DataFiles.CopyTo(GoodDataFiles, 0)

        Return GoodDataFiles
    End Function



    '*******************************************************************
    '*   Read the data file directory, File 0, Type 2
    '*    Then extract the number of data and program files
    '*******************************************************************
    Private Function GetML1500DataMemory() As DataFileDetails()
        Dim reply As Integer
        Dim PAddress As ParsedDataAddress

        '* Get the length of File 0, Type 2. This is the program/data file directory
        PAddress.FileNumber = 0
        PAddress.FileType = 2
        PAddress.Element = &H2F
        Dim data() As Byte = ReadRawData(PAddress, 2, reply)


        If reply = 0 Then
            Dim FileZeroSize As Integer = data(0) + (data(1)) * 256

            PAddress.Element = 0
            PAddress.SubElement = 0
            '* Read all of File 0, Type 2
            Dim FileZeroData() As Byte = ReadRawData(PAddress, FileZeroSize, reply)

            '* Start Reading the data table configuration
            Dim DataFiles(256) As DataFileDetails

            Dim FilePosition As Integer
            Dim i As Integer


            '* Process the data from the data table
            If reply = 0 Then
                '* Comb through data file 0 looking for data table definitions
                Dim k, BytesPerElement As Integer
                i = 0
                FilePosition = 143
                While FilePosition < FileZeroData.Length
                    Select Case FileZeroData(FilePosition)
                        Case &H89 : DataFiles(k).FileType = "N"
                            BytesPerElement = 2
                        Case &H85 : DataFiles(k).FileType = "B"
                            BytesPerElement = 2
                        Case &H86 : DataFiles(k).FileType = "T"
                            BytesPerElement = 6
                        Case &H87 : DataFiles(k).FileType = "C"
                            BytesPerElement = 6
                        Case &H84 : DataFiles(k).FileType = "S"
                            BytesPerElement = 2
                        Case &H8A : DataFiles(k).FileType = "F"
                            BytesPerElement = 4
                        Case &H8D : DataFiles(k).FileType = "ST"
                            BytesPerElement = 84
                        Case &H8E : DataFiles(k).FileType = "A"
                            BytesPerElement = 2
                        Case &H88 : DataFiles(k).FileType = "R"
                            BytesPerElement = 6
                        Case &H82, &H8B : DataFiles(k).FileType = "O"
                            BytesPerElement = 2
                        Case &H83, &H8C : DataFiles(k).FileType = "I"
                            BytesPerElement = 2
                        Case &H91 : DataFiles(k).FileType = "L"   'Long Integer
                            BytesPerElement = 4
                        Case &H92 : DataFiles(k).FileType = "MG"   'Message Command 146
                            BytesPerElement = 50
                        Case &H93 : DataFiles(k).FileType = "PD"   'PID
                            BytesPerElement = 46
                        Case &H94 : DataFiles(k).FileType = "PLS"   'Programmable Limit Swith
                            BytesPerElement = 12

                        Case Else : DataFiles(k).FileType = "Undefined"  '* 61h=Program File
                            BytesPerElement = 2
                    End Select
                    DataFiles(k).NumberOfElements = (FileZeroData(FilePosition + 1) + FileZeroData(FilePosition + 2) * 256) / BytesPerElement
                    DataFiles(k).FileNumber = i

                    '* Only return valid user data files
                    If FileZeroData(FilePosition) > &H81 And FileZeroData(FilePosition) < &H95 Then k += 1

                    '* Index file number once in the region of data files
                    If k > 0 Then i += 1
                    FilePosition += 10
                End While

                '* Move to an array with a length of only good data files
                Dim GoodDataFiles(k - 1) As DataFileDetails
                For l As Integer = 0 To k - 1
                    GoodDataFiles(l) = DataFiles(l)
                Next

                Return GoodDataFiles
            Else
                Throw New DF1Exception(DecodeMessage(reply) & " - Failed to get data table list")
            End If
        Else
            Throw New DF1Exception(DecodeMessage(reply) & " - Failed to get data table list")
        End If
    End Function

    Private Function ReadFileDirectory() As Byte()
        GetProcessorType()

        '*****************************************************
        '* 1 & 2) Get the size of the File Directory
        '*****************************************************
        Dim PAddress As ParsedDataAddress
        Select Case ProcessorType
            Case &H25, &H58  '* SLC 5/02 or ML1000
                PAddress.FileType = 0
                PAddress.Element = &H23
            Case &H88, &H89, &H8C, &H9C  '* ML1100, ML1200, ML1500
                PAddress.FileType = 2
                PAddress.Element = &H2F
            Case Else           '* SLC 5/04, SLC 5/05
                PAddress.FileType = 1
                PAddress.Element = &H23
        End Select

        Dim reply As Integer

        Dim data() As Byte = ReadRawData(PAddress, 2, reply)
        If reply <> 0 Then Throw New DF1Exception("Failed to Get Program Directory Size- " & DecodeMessage(reply))


        '*****************************************************
        '* 3) Read All of File 0 (File Directory)
        '*****************************************************
        PAddress.Element = 0
        Dim FileZeroSize As Integer = data(0) + data(1) * 256
        Dim FileZeroData() As Byte = ReadRawData(PAddress, FileZeroSize, reply)
        If reply <> 0 Then Throw New DF1Exception("Failed to Get Program Directory - " & DecodeMessage(reply))

        Return FileZeroData
    End Function
    '********************************************************************
    '* Retreive the ladder files
    '* This was developed from a combination of Chapter 12
    '*  and reverse engineering
    '********************************************************************
    Public Structure PLCFileDetails
        Dim FileType As Integer
        Dim FileNumber As Integer
        Dim NumberOfBytes As Integer
        Dim data() As Byte
    End Structure
    Public Function UploadProgramData() As System.Collections.ObjectModel.Collection(Of PLCFileDetails)
        ''*****************************************************
        ''* 1,2 & 3) Read all of the directory File
        ''*****************************************************
        Dim FileZeroData() As Byte = ReadFileDirectory()

        Dim PAddress As ParsedDataAddress
        Dim reply As Integer

        RaiseEvent UploadProgress(Me, System.EventArgs.Empty)

        '**************************************************
        '* 4) Parse the data from the File Directory data
        '**************************************************
        '*********************************************************************************
        '* Starting at corresponding File Position, each program is defined with 10 bytes
        '* 1st byte=File Type
        '* 2nd & 3rd bytes are program size
        '* 4th & 5th bytes are location with memory
        '*********************************************************************************
        Dim FilePosition As Integer
        Dim ProgramFile As New PLCFileDetails
        Dim ProgramFiles As New System.Collections.ObjectModel.Collection(Of PLCFileDetails)

        '*********************************************
        '* 4a) Add the directory information
        '*********************************************
        ProgramFile.FileNumber = 0
        ProgramFile.data = FileZeroData
        ProgramFile.FileType = PAddress.FileType
        ProgramFile.NumberOfBytes = FileZeroData.Length
        ProgramFiles.Add(ProgramFile)

        '**********************************************
        '* 5) Read the rest of the data tables
        '**********************************************
        Dim DataFileGroup, ForceFileGroup, SystemFileGroup, SystemLadderFileGroup As Integer
        Dim LadderFileGroup, Unknown1FileGroup, Unknown2FileGroup As Integer
        If reply = 0 Then
            Dim NumberOfProgramFiles As Integer = FileZeroData(46) + FileZeroData(47) * 256

            '* Comb through data file 0 and get the program file details
            Dim i As Integer
            '* The start of program file definitions
            Select Case ProcessorType
                Case &H25, &H58
                    FilePosition = 93
                Case &H88, &H89, &H8C, &H9C
                    FilePosition = 103
                Case Else
                    FilePosition = 79
            End Select

            Do While FilePosition < FileZeroData.Length And reply = 0
                ProgramFile.FileType = FileZeroData(FilePosition)
                ProgramFile.NumberOfBytes = (FileZeroData(FilePosition + 1) + FileZeroData(FilePosition + 2) * 256)

                If ProgramFile.FileType >= &H40 AndAlso ProgramFile.FileType <= &H5F Then
                    ProgramFile.FileNumber = SystemFileGroup
                    SystemFileGroup += 1
                End If
                If (ProgramFile.FileType >= &H20 AndAlso ProgramFile.FileType <= &H3F) Then
                    ProgramFile.FileNumber = LadderFileGroup
                    LadderFileGroup += 1
                End If
                If (ProgramFile.FileType >= &H60 AndAlso ProgramFile.FileType <= &H7F) Then
                    ProgramFile.FileNumber = SystemLadderFileGroup
                    SystemLadderFileGroup += 1
                End If
                If ProgramFile.FileType >= &H80 AndAlso ProgramFile.FileType <= &H9F Then
                    ProgramFile.FileNumber = DataFileGroup
                    DataFileGroup += 1
                End If
                If ProgramFile.FileType >= &HA0 AndAlso ProgramFile.FileType <= &HBF Then
                    ProgramFile.FileNumber = ForceFileGroup
                    ForceFileGroup += 1
                End If
                If ProgramFile.FileType >= &HC0 AndAlso ProgramFile.FileType <= &HDF Then
                    ProgramFile.FileNumber = Unknown1FileGroup
                    Unknown1FileGroup += 1
                End If
                If ProgramFile.FileType >= &HE0 AndAlso ProgramFile.FileType <= &HFF Then
                    ProgramFile.FileNumber = Unknown2FileGroup
                    Unknown2FileGroup += 1
                End If

                PAddress.FileType = ProgramFile.FileType
                PAddress.FileNumber = ProgramFile.FileNumber

                If ProgramFile.NumberOfBytes > 0 Then
                    ProgramFile.data = ReadRawData(PAddress, ProgramFile.NumberOfBytes, reply)
                    If reply <> 0 Then Throw New DF1Exception("Failed to Read Program File " & PAddress.FileNumber & ", Type " & PAddress.FileType & " - " & DecodeMessage(reply))
                Else
                    Dim ZeroLengthData(-1) As Byte
                    ProgramFile.data = ZeroLengthData
                End If


                ProgramFiles.Add(ProgramFile)
                RaiseEvent UploadProgress(Me, System.EventArgs.Empty)

                i += 1
                '* 10 elements are used to define each program file
                '* SLC 5/02 or ML1000
                If ProcessorType = &H25 OrElse ProcessorType = &H58 Then
                    FilePosition += 8
                Else
                    FilePosition += 10
                End If
            Loop

        End If

        Return ProgramFiles
    End Function

    '****************************************************************
    '* Download a group of files defined in the PLCFiles Collection
    '****************************************************************
    Public Sub DownloadProgramData(ByVal PLCFiles As System.Collections.ObjectModel.Collection(Of PLCFileDetails))
        '******************************
        '* 1 & 2) Change to program Mode
        '******************************
        SetProgramMode()
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)

        '*************************************************************************
        '* 2) Initialize Memory & Put in Download mode using Execute Command List
        '*************************************************************************
        Dim DataLength As Integer
        Select Case ProcessorType
            Case &H5B, &H78
                DataLength = 13
            Case &H88, &H89, &H8C, &H9C
                DataLength = 15
            Case Else
                DataLength = 15
        End Select

        Dim data(DataLength) As Byte
        '* 2 commands
        data(0) = &H2
        '* Number of bytes in 1st command
        data(1) = &HA
        '* Function &HAA
        data(2) = &HAA
        '* Write 4 bytes
        data(3) = 4
        data(4) = 0
        '* File type 63
        data(5) = &H63

        '* Lets go ahead and setup the file type for later use
        Dim PAddress As ParsedDataAddress
        Dim reply As Integer

        '**********************************
        '* 2a) Search for File 0, Type 24
        '**********************************
        Dim i As Integer
        While i < PLCFiles.Count AndAlso (PLCFiles(i).FileNumber <> 0 OrElse PLCFiles(i).FileType <> &H24)
            i += 1
        End While

        '* Write bytes 02-07 from File 0, Type 24 to File 0, Type 63
        If i < PLCFiles.Count Then
            data(8) = PLCFiles(i).data(2)
            data(9) = PLCFiles(i).data(3)
            data(10) = PLCFiles(i).data(4)
            data(11) = PLCFiles(i).data(5)
            If DataLength > 14 Then
                data(12) = PLCFiles(i).data(6)
                data(13) = PLCFiles(i).data(7)
            End If
        End If


        Select Case ProcessorType
            Case &H78, &H5B, &H49  '* SLC 5/05, 5/04, 5/03
                '* Read these 4 bytes to write back, File 0, Type 63
                PAddress.FileType = &H63
                PAddress.Element = 0
                PAddress.SubElement = 0
                Dim FourBytes() As Byte = ReadRawData(PAddress, 4, reply)
                If reply = 0 Then
                    Array.Copy(FourBytes, 0, data, 8, 4)
                    PAddress.FileType = 1
                    PAddress.Element = &H23
                Else
                    Throw New DF1Exception("Failed to Read File 0, Type 63h - " & DecodeMessage(reply))
                End If

                '* Number of bytes in 1st command
                data(1) = &HA
                '* Number of bytes to write
                data(3) = 4
            Case &H88, &H89, &H8C, &H9C   '* ML1200, ML1500, ML1100
                '* Number of bytes in 1st command
                data(1) = &HC
                '* Number of bytes to write
                data(3) = 6
                PAddress.FileType = 2
                PAddress.Element = &H23
            Case Else '* Fill in the gap for an unknown processor
                data(1) = &HA
                data(3) = 4
                PAddress.FileType = 1
                PAddress.Element = &H23
        End Select


        '* 1 byte in 2nd command - Start Download
        data(data.Length - 2) = 1
        data(data.Length - 1) = &H56

        Dim rTNS As Integer
        reply = PrefixAndSend(&HF, &H88, data, True, rTNS)
        If reply <> 0 Then Throw New DF1Exception("Failed to Initialize for Download - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)


        '*********************************
        '* 4) Secure Sole Access
        '*********************************
        Dim data2(-1) As Byte
        reply = PrefixAndSend(&HF, &H11, data2, True, rTNS)
        If reply <> 0 Then Throw New DF1Exception("Failed to Secure Sole Access - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)

        '*********************************
        '* 5) Write the directory length
        '*********************************
        PAddress.BitNumber = 16
        Dim data3(1) As Byte
        data3(0) = PLCFiles(0).data.Length And &HFF
        data3(1) = (PLCFiles(0).data.Length - data3(0)) / 256
        reply = WriteRawData(PAddress, 2, data3)
        If reply <> 0 Then Throw New DF1Exception("Failed to Write Directory Length - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)

        '*********************************
        '* 6) Write program directory
        '*********************************
        PAddress.Element = 0
        reply = WriteRawData(PAddress, PLCFiles(0).data.Length, PLCFiles(0).data)
        If reply <> 0 Then Throw New DF1Exception("Failed to Write New Program Directory - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)

        '*********************************
        '* 7) Write Program & Data Files
        '*********************************
        For i = 1 To PLCFiles.Count - 1
            PAddress.FileNumber = PLCFiles(i).FileNumber
            PAddress.FileType = PLCFiles(i).FileType
            PAddress.Element = 0
            PAddress.SubElement = 0
            PAddress.BitNumber = 16
            reply = WriteRawData(PAddress, PLCFiles(i).data.Length, PLCFiles(i).data)
            If reply <> 0 Then Throw New DF1Exception("Failed when writing files to PLC - " & DecodeMessage(reply))
            RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)
        Next

        '*********************************
        '* 8) Complete the Download
        '*********************************
        reply = PrefixAndSend(&HF, &H52, data2, True, rTNS)
        If reply <> 0 Then Throw New DF1Exception("Failed to Indicate to PLC that Download is complete - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)

        '*********************************
        '* 9) Release Sole Access
        '*********************************
        reply = PrefixAndSend(&HF, &H12, data2, True, rTNS)
        If reply <> 0 Then Throw New DF1Exception("Failed to Release Sole Access - " & DecodeMessage(reply))
        RaiseEvent DownloadProgress(Me, System.EventArgs.Empty)
    End Sub


    ''' <summary>
    ''' Get the number of slots in the rack
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSlotCount() As Integer
        '* Get the header of the data table definition file
        Dim data(4) As Byte

        '* Number of bytes to read
        data(0) = 4
        '* Data File Number (0 is the system file)
        data(1) = 0
        '* File Type (&H60 must be a system type), this was pulled from reverse engineering
        data(2) = &H60
        '* Element Number
        data(3) = 0
        '* Sub Element Offset in words
        data(4) = 0


        Dim rTNS As Integer
        Dim reply As Integer = PrefixAndSend(&HF, &HA2, data, True, rTNS)

        If reply = 0 Then
            If DataPackets(rTNS)(6) > 0 Then
                Return DataPackets(rTNS)(6) - 1  '* if a rack based system, then subtract processor slot
            Else
                Return 0  '* micrologix reports 0 slots
            End If
        Else
            Throw New DF1Exception("Failed to Release Sole Access - " & DecodeMessage(reply))
        End If
    End Function

    Public Structure IOConfig
        Dim InputBytes As Integer
        Dim OutputBytes As Integer
        Dim CardCode As Integer
    End Structure
    ''' <summary>
    ''' Get IO card list currently in rack
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetIOConfig() As IOConfig()
        Dim ProcessorType As Integer = GetProcessorType()


        If ProcessorType = &H89 Or ProcessorType = &H8C Then  '* Is it a Micrologix 1500?
            Return GetML1500IOConfig()
        Else
            Return GetSLCIOConfig()
        End If
    End Function

    ''' <summary>
    ''' Get IO card list currently in rack of a SLC
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetSLCIOConfig() As IOConfig()
        Dim slots As Integer = GetSlotCount()

        If slots > 0 Then
            '* Get the header of the data table definition file
            Dim data(4) As Byte

            '* Number of bytes to read
            data(0) = 4 + (slots + 1) * 6 + 2
            '* Data File Number (0 is the system file)
            data(1) = 0
            '* File Type (&H60 must be a system type), this was pulled from reverse engineering
            data(2) = &H60
            '* Element Number
            data(3) = 0
            '* Sub Element Offset in words
            data(4) = 0


            Dim rTNS As Integer
            Dim reply As Integer = PrefixAndSend(&HF, &HA2, data, True, rTNS)

            Dim BytesForConverting(1) As Byte
            Dim IOResult(slots) As IOConfig
            If reply = 0 Then
                '* Extract IO information
                For i As Integer = 0 To slots
                    IOResult(i).InputBytes = DataPackets(rTNS)(i * 6 + 10)
                    IOResult(i).OutputBytes = DataPackets(rTNS)(i * 6 + 12)
                    BytesForConverting(0) = DataPackets(rTNS)(i * 6 + 14)
                    BytesForConverting(1) = DataPackets(rTNS)(i * 6 + 15)
                    IOResult(i).CardCode = BitConverter.ToInt16(BytesForConverting, 0)
                Next
                Return IOResult
            Else
                Throw New DF1Exception("Failed to get IO Config - " & DecodeMessage(reply))
            End If
        Else
            Throw New DF1Exception("Failed to get Slot Count")
        End If
    End Function


    ''' <summary>
    ''' Get IO card list currently in rack of a ML1500
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetML1500IOConfig() As IOConfig()
        '*************************************************************************
        '* Read the first 4 bytes of File 0, type 62 to get the total file length
        '**************************************************************************
        Dim data(4) As Byte
        Dim rTNS As Integer

        '* Number of bytes to read
        data(0) = 4
        '* Data File Number (0 is the system file)
        data(1) = 0
        '* File Type (&H62 must be a system type), this was pulled from reverse engineering
        data(2) = &H62
        '* Element Number
        data(3) = 0
        '* Sub Element Offset in words
        data(4) = 0

        Dim reply As Integer = PrefixAndSend(&HF, &HA2, data, True, rTNS)

        '******************************************
        '* Read all of File Zero, Type 62
        '******************************************
        If reply = 0 Then
            'TODO: Get this corrected
            Dim FileZeroSize As Integer = DataPackets(rTNS)(6) * 2
            Dim FileZeroData(FileZeroSize) As Byte
            Dim FilePosition As Integer
            Dim Subelement As Integer
            Dim i As Integer

            '* Number of bytes to read
            If FileZeroSize > &H50 Then
                data(0) = &H50
            Else
                data(0) = FileZeroSize
            End If

            '* Loop through reading all of file 0 in chunks of 80 bytes
            Do While FilePosition < FileZeroSize And reply = 0

                '* Read the file
                reply = PrefixAndSend(&HF, &HA2, data, True, rTNS)

                '* Transfer block of data read to the data table array
                i = 0
                Do While i < data(0)
                    FileZeroData(FilePosition) = DataPackets(rTNS)(i + 6)
                    i += 1
                    FilePosition += 1
                Loop


                '* point to the next element, by taking the last Start Element(in words) and adding it to the number of bytes read
                Subelement += data(0) / 2
                If Subelement < 255 Then
                    data(3) = Subelement
                Else
                    '* Use extended addressing
                    If data.Length < 6 Then ReDim Preserve data(5)
                    data(5) = Math.Floor(Subelement / 256)  '* 256+data(5)
                    data(4) = Subelement - (data(5) * 256) '*  calculate offset
                    data(3) = 255
                End If

                '* Set next length of data to read. Max of 80
                If FileZeroSize - FilePosition < 80 Then
                    data(0) = FileZeroSize - FilePosition
                Else
                    data(0) = 80
                End If
            Loop


            '**********************************
            '* Extract the data from the file
            '**********************************
            If reply = 0 Then
                Dim SlotCount As Integer = FileZeroData(2) - 2
                If SlotCount < 0 Then SlotCount = 0
                Dim SlotIndex As Integer = 1
                Dim IOResult(SlotCount) As IOConfig

                '*Start getting slot data
                i = 32 + SlotCount * 4
                Dim BytesForConverting(1) As Byte

                Do While SlotIndex <= SlotCount
                    IOResult(SlotIndex).InputBytes = FileZeroData(i + 2) * 2
                    IOResult(SlotIndex).OutputBytes = FileZeroData(i + 8) * 2
                    BytesForConverting(0) = FileZeroData(i + 18)
                    BytesForConverting(1) = FileZeroData(i + 19)
                    IOResult(SlotIndex).CardCode = BitConverter.ToInt16(BytesForConverting, 0)

                    i += 26
                    SlotIndex += 1
                Loop


                '****************************************
                '* Get the Slot 0(base unit) information
                '****************************************
                data(0) = 8
                '* Data File Number (0 is the system file)
                data(1) = 0
                '* File Type (&H60 must be a system type), this was pulled from reverse engineering
                data(2) = &H60
                '* Element Number
                data(3) = 0
                '* Sub Element Offset in words
                data(4) = 0


                '* Read File 0 to get the IO count on the base unit
                reply = PrefixAndSend(&HF, &HA2, data, True, rTNS)

                If reply = 0 Then
                    IOResult(0).InputBytes = DataPackets(rTNS)(10)
                    IOResult(0).OutputBytes = DataPackets(rTNS)(12)
                Else
                    Throw New DF1Exception("Failed to get Base IO Config for Micrologix 1500- " & DecodeMessage(reply))
                End If


                Return IOResult
            End If
        End If

        Throw New DF1Exception("Failed to get IO Config for Micrologix 1500- " & DecodeMessage(reply))
    End Function


    '***************************************************************
    '* This method is intended to make it easy to configure the
    '* comm port settings. It is similar to the auto configure
    '* in RSLinx.
    '* It uses the echo command and sends the character "A", then
    '* checks if it received a response.
    '**************************************************************
    ''' <summary>
    ''' This method is intended to make it easy to configure the
    ''' comm port settings. It is similar to the auto configure
    ''' in RSLinx. A successful configuration returns a 0 and sets the
    ''' properties to the discovered values.
    ''' It will fire the event "AutoDetectTry" for each setting attempt
    ''' It uses the echo command and sends the character "A", then
    ''' checks if it received a response.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DetectCommSettings() As Integer
        'Dim rTNS As Integer

        Dim data() As Byte = {65}
        Dim BaudRates() As Integer = {38400, 19200, 9600}
        Dim BRIndex As Integer = 0
        Dim Parities() As System.IO.Ports.Parity = {System.IO.Ports.Parity.None, System.IO.Ports.Parity.Even}
        Dim PIndex As Integer
        Dim Checksums() As CheckSumOptions = {CheckSumOptions.Crc, CheckSumOptions.Bcc}
        Dim CSIndex As Integer
        Dim reply As Integer = -1

        DisableEvent = True
        '* We are sending a small amount of data, so speed up the response
        MaxTicks = 3
        While BRIndex < BaudRates.Length And reply <> 0
            PIndex = 0
            While PIndex < Parities.Length And reply <> 0
                CSIndex = 0
                While CSIndex < Checksums.Length And reply <> 0
                    CloseComms()
                    m_BaudRate = BaudRates(BRIndex)
                    m_Parity = Parities(PIndex)
                    m_CheckSum = Checksums(CSIndex)

                    RaiseEvent AutoDetectTry(Me, System.EventArgs.Empty)

                    '* send an "A" and look for echo back
                    'reply = PrefixAndSend(&H6, &H0, data, True, rTNS)

                    '* Send an ENQ sequence until we get a reply
                    reply = SendENQ()

                    '* If port cannot be opened, do not retry
                    If reply = -6 Then Return reply

                    MaxTicks += 1
                    CSIndex += 1
                End While
                PIndex += 1
            End While
            BRIndex += 1
        End While

        DisableEvent = False
        MaxTicks = 100
        Return reply
    End Function

    '******************************************
    '* Synchronous read of any data type
    '*  this function does not declare its return type because it dependent on the data type read
    '******************************************
    ''' <summary>
    ''' Synchronous read of any data type
    ''' this function returns results as an array of strings
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="numberOfElements"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadAny(ByVal startAddress As String, ByVal numberOfElements As Integer) As String()
        '* Limit number of elements to one complete file or 256 elements
        '* NOT TRUE
        'If numberOfElements > 256 Then
        'Throw New ApplicationException("Can not read more than 256 elements")
        'numberOfElements = 256
        'End If

        Dim data(4) As Byte
        Dim ParsedResult As ParsedDataAddress = ParseAddress(startAddress)

        '* Invalid address?
        If ParsedResult.FileType = 0 Then
            Throw New DF1Exception("Invalid Address")
        End If


        '* If requesting 0 elements, then default to 1
        Dim ArrayElements As Int16 = numberOfElements - 1
        If ArrayElements < 0 Then
            ArrayElements = 0
        End If

        '* If reading at bit level ,then convert number bits to read to number of words
        If ParsedResult.BitNumber < 16 Then
            ArrayElements = Math.Floor(numberOfElements / 16)
            If ArrayElements Mod 16 > 0 Then data(0) += 1
        End If


        '* Number of bytes to read
        Dim NumberOfBytes As Integer
        'NumberOfBytes = ((ArrayElements + 1)) * 2

        Select Case ParsedResult.FileType
            Case &H8D : NumberOfBytes = ((ArrayElements + 1)) * 82  '* String
            Case &H8A : NumberOfBytes = ((ArrayElements + 1)) * 4   '* Float
            Case &H91 : NumberOfBytes = ((ArrayElements + 1)) * 4   '* Long
            Case &H92 : NumberOfBytes = ((ArrayElements + 1)) * 50  '* Message
            Case &H86, &H87 : NumberOfBytes = ((ArrayElements + 1)) * 2   '* Timer
                'ArrayElements = (ArrayElements + 1) * 3 - 1
            Case Else : NumberOfBytes = ((ArrayElements + 1)) * 2
        End Select


        '* If it is a multiple read of sub-elements of timers and counter, then read an array of the same consectutive sub element
        If ParsedResult.SubElement > 0 AndAlso (ParsedResult.FileType = &H86 Or ParsedResult.FileType = &H87) Then
            NumberOfBytes = (NumberOfBytes * 3) - 4  '* There are 3 words per sub element (6 bytes)
        End If


        Dim reply As Integer
        Dim ReturnedData(NumberOfBytes) As Byte
        Dim ReturnedDataIndex As Integer

        Dim BytesToRead As Integer

        Dim Retries As Integer
        While reply = 0 AndAlso ReturnedDataIndex < NumberOfBytes
            BytesToRead = NumberOfBytes

            Dim ReturnedData2(BytesToRead) As Byte
            ReturnedData2 = ReadRawData(ParsedResult, BytesToRead, reply)

            '* Point to next set of bytes to read in block
            If reply = 0 Then
                '* Point to next element to begin reading
                ReturnedData2.CopyTo(ReturnedData, ReturnedDataIndex)
                ReturnedDataIndex += BytesToRead

            ElseIf Retries < 2 Then
                Retries += 1
                reply = 0
            Else
                '* An error was returned from the read operation
                Throw New DF1Exception(DecodeMessage(reply))
            End If
        End While


        'If reply = 0 Then
        '***************************************************
        '* Extract returned data into appropriate data type
        '***************************************************
        Dim result(ArrayElements) As String
        Dim StringLength As Integer
        'Dim StringResult(ArrayElements) As String
        Select Case ParsedResult.FileType
            Case &H8A '* Floating point read (&H8A)
                'Dim result(ArrayElements) As Single
                For i As Integer = 0 To ArrayElements
                    result(i) = BitConverter.ToSingle(ReturnedData, (i * 4))
                Next
            Case &H8D ' * String
                'Dim result(ArrayElements) As String
                For i As Integer = 0 To ArrayElements
                    'result(i) = BitConverter.ToString(ReturnedData, 2, StringLength)
                    StringLength = BitConverter.ToInt16(ReturnedData, (i * 84))
                    '* The controller may falsely report the string length, so set to max allowed
                    If StringLength > 82 Then StringLength = 82

                    '* use a string builder for increased performance
                    Dim result2 As New System.Text.StringBuilder
                    Dim j As Integer = 2
                    '* Stop concatenation if a zero (NULL) is reached
                    While j < StringLength + 2 And ReturnedData((i * 84) + j + 1) > 0
                        result2.Append(CStr(ReturnedData((i * 84) + j + 1)))
                        '* Prevent an odd length string from getting a Null added on
                        If j < StringLength + 1 And (ReturnedData((i * 84) + j)) > 0 Then result2.Append(CStr(ReturnedData((i * 84) + j)))
                        j += 2
                    End While
                    result(i) = result2.ToString
                Next
            Case &H86, &H87  '* Timer, counter
                '* If a sub element is designated then read the same sub element for all timers
                Dim j As Integer
                For i As Integer = 0 To ArrayElements
                    If ParsedResult.SubElement > 0 Then
                        j = i * 6
                    Else
                        j = i * 2
                    End If
                    result(i) = BitConverter.ToInt16(ReturnedData, (j))
                Next
            Case &H91 '* Long Value read (&H91)
                'Dim result(ArrayElements) As Single
                For i As Integer = 0 To ArrayElements
                    result(i) = BitConverter.ToInt32(ReturnedData, (i * 4))
                Next
            Case &H92 '* MSG Value read (&H92)
                'Dim result(ArrayElements) As Single
                For i As Integer = 0 To ArrayElements
                    result(i) = BitConverter.ToString(ReturnedData, (i * 50), 50)
                Next
            Case Else
                'Dim result(ArrayElements) As Int16
                For i As Integer = 0 To ArrayElements
                    result(i) = BitConverter.ToInt16(ReturnedData, (i * 2))
                Next
        End Select
        'End If


        '******************************************************************************
        '* If the number of words to read is not specified, then return a single value
        '******************************************************************************
        '* Is it a bit level and N or B file?
        If ParsedResult.BitNumber >= 0 And ParsedResult.BitNumber < 16 Then
            Dim BitResult(numberOfElements - 1) As String
            Dim BitPos As Integer = ParsedResult.BitNumber
            Dim WordPos As Integer = 0
            'Dim Result(ArrayElements) As Boolean
            '* Set array of consectutive bits
            For i As Integer = 0 To numberOfElements - 1
                BitResult(i) = CBool(result(WordPos) And 2 ^ BitPos)
                BitPos += 1
                If BitPos > 15 Then
                    BitPos = 0
                    WordPos += 1
                End If
            Next
            Return BitResult
        End If

        Return result

        '* An error must have occurred if it made it this far, so throw exception
        'Throw New DF1Exception(DecodeMessage(reply))
    End Function
    '*************************************************************
    '* Overloaded method of ReadAny - that reads only one element
    '*************************************************************
    ''' <summary>
    ''' Synchronous read of any data type
    ''' this function returns results as a string
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadAny(ByVal startAddress As String) As String
        Return ReadAny(startAddress, 1)(0)
    End Function

    ''' <summary>
    ''' Reads values and returns them as integers
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="numberOfBytes"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ReadInt(ByVal startAddress As String, ByVal numberOfBytes As Integer) As Integer()
        Dim result() As String
        result = ReadAny(startAddress, numberOfBytes)

        Dim Ints(result.Length) As Integer
        For i As Integer = 0 To result.Length - 1
            Ints(i) = CInt(result(i))
        Next

        Return Ints
    End Function

    '*********************************************************************************
    '* Read Raw File data and break up into chunks because of limits of DF1 protocol
    '*********************************************************************************
    Private Function ReadRawData(ByVal PAddress As ParsedDataAddress, ByVal numberOfBytes As Integer, ByRef reply As Integer) As Byte()
        Dim NumberOfBytesToRead, FilePosition, rTNS As Integer
        Dim ResultData(numberOfBytes - 1) As Byte

        Do While FilePosition < numberOfBytes AndAlso reply = 0
            '* Set next length of data to read. Max of 236 (slc 5/03 and up)
            '* This must limit to 82 for 5/02 and below
            If numberOfBytes - FilePosition < 236 Then
                NumberOfBytesToRead = numberOfBytes - FilePosition
            Else
                NumberOfBytesToRead = 236
            End If

            '* String is an exception
            If NumberOfBytesToRead > 168 AndAlso PAddress.FileType = &H8D Then
                '* Only two string elements can be read on each read (168 bytes)
                NumberOfBytesToRead = 168
            End If

            If NumberOfBytesToRead > 234 AndAlso (PAddress.FileType = &H86 OrElse PAddress.FileType = &H87) Then
                '* Timers & counters read in multiples of 6 bytes
                NumberOfBytesToRead = 234
            End If

            '* Data Monitor File is an exception
            If NumberOfBytesToRead > &H78 AndAlso PAddress.FileType = &HA4 Then
                '* Only two string elements can be read on each read (168 bytes)
                NumberOfBytesToRead = &H78
            End If

            '* The SLC 5/02 can only read &H50 bytes per read, possibly the ML1500
            'If NumberOfBytesToRead > &H50 AndAlso (ProcessorType = &H25 Or ProcessorType = &H89) Then
            If NumberOfBytesToRead > &H50 AndAlso (ProcessorType = &H25) Then
                NumberOfBytesToRead = &H50
            End If

            If NumberOfBytesToRead > 0 Then
                Dim DataSize, Func As Integer

                If PAddress.SubElement = 0 Then
                    DataSize = 3
                    Func = &HA1
                Else
                    DataSize = 4
                    Func = &HA2
                End If

                '* Check if we need extended addressing
                If PAddress.Element >= 255 Then DataSize += 2
                If PAddress.SubElement >= 255 Then DataSize += 2

                Dim data(DataSize) As Byte


                '* Number of bytes to read - 
                data(0) = NumberOfBytesToRead

                '* File Number
                data(1) = PAddress.FileNumber

                '* File Type
                data(2) = PAddress.FileType

                '* Starting Element Number
                '* point to the next element (ref page 7-17)
                If PAddress.Element < 255 Then
                    data(3) = PAddress.Element
                Else
                    '* Use extended addressing
                    data(5) = Math.Floor(PAddress.Element / 256)  '* 256+data(5)
                    data(4) = PAddress.Element - (data(5) * 256) '*  calculate offset
                    data(3) = 255
                End If

                '* Sub Element (Are we using the subelement function of &HA2?)
                If Func = &HA2 Then
                    '* point to the next element (ref page 7-17)
                    If PAddress.SubElement < 255 Then
                        data(data.Length - 1) = PAddress.SubElement
                    Else
                        '* Use extended addressing
                        data(data.Length - 1) = Math.Floor(PAddress.SubElement / 256)
                        data(data.Length - 2) = PAddress.SubElement - (data(data.Length - 1) * 256)
                        data(data.Length - 3) = 255
                    End If
                End If


                reply = PrefixAndSend(&HF, Func, data, True, rTNS)

                '***************************************************
                '* Extract returned data into appropriate data type
                '* Transfer block of data read to the data table array
                '***************************************************
                '* TODO: Check array bounds
                'Array.Copy(data, 6, ResultData, FilePosition, NumberOfBytesToRead)
                If reply = 0 Then
                    For i As Integer = 0 To NumberOfBytesToRead - 1
                        ResultData(FilePosition + i) = DataPackets(rTNS)(i + 6)
                    Next
                End If

                FilePosition += NumberOfBytesToRead

                '* point to the next element
                If PAddress.FileType = &HA4 Then
                    PAddress.Element += NumberOfBytesToRead / &H28
                Else
                    '* Use subelement because it works with all data file types
                    PAddress.SubElement += NumberOfBytesToRead / 2
                End If
            End If
        Loop

        Return ResultData
    End Function



    '*****************************************************************
    '* Write Section
    '*
    '* Address is in the form of <file type><file Number>:<offset>
    '* examples  N7:0, B3:0,
    '******************************************************************

    '* Handle one value of Integer type
    ''' <summary>
    ''' Write a single integer value to a PLC data table
    ''' The startAddress is in the common form of AB addressing (e.g. N7:0)
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="dataToWrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteData(ByVal startAddress As String, ByVal dataToWrite As Integer) As String
        Dim temp(1) As Integer
        temp(0) = dataToWrite
        Return WriteData(startAddress, 1, temp)
    End Function


    '* Write an array of integers
    ''' <summary>
    ''' Write multiple consectutive integer values to a PLC data table
    ''' The startAddress is in the common form of AB addressing (e.g. N7:0)
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="numberOfElements"></param>
    ''' <param name="dataToWrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteData(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As Integer) As Integer
        Dim ParsedResult As ParsedDataAddress = ParseAddress(startAddress)

        Dim ConvertedData(numberOfElements * ParsedResult.BytesPerElements) As Byte

        Dim i As Integer
        If ParsedResult.FileType = &H91 Then
            '* Write to a Long integer file
            While i < numberOfElements
                '******* NOT Necesary to validate because dataToWrite keeps it in range for a long
                Dim b(3) As Byte
                b = BitConverter.GetBytes(dataToWrite(i))

                ConvertedData(i * 4) = b(0)
                ConvertedData(i * 4 + 1) = b(1)
                ConvertedData(i * 4 + 2) = b(2)
                ConvertedData(i * 4 + 3) = b(3)
                i += 1
            End While
        Else
            While i < numberOfElements
                '* Validate range
                If dataToWrite(i) > 32767 Or dataToWrite(i) < -32768 Then
                    Throw New DF1Exception("Integer data out of range, must be between -32768 and 32767")
                End If

                ConvertedData(i * 2) = CByte(dataToWrite(i) And &HFF)
                ConvertedData(i * 2 + 1) = CByte((dataToWrite(i) >> 8) And &HFF)

                i += 1
            End While
        End If

        Return WriteRawData(ParsedResult, numberOfElements * ParsedResult.BytesPerElements, ConvertedData)
    End Function

    '* Handle one value of Single type
    ''' <summary>
    ''' Write a single floating point value to a data table
    ''' The startAddress is in the common form of AB addressing (e.g. F8:0)
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="dataToWrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteData(ByVal startAddress As String, ByVal dataToWrite As Single) As Integer
        Dim temp(1) As Single
        temp(0) = dataToWrite
        Return WriteData(startAddress, 1, temp)
    End Function

    '* Write an array of Singles
    ''' <summary>
    ''' Write multiple consectutive floating point values to a PLC data table
    ''' The startAddress is in the common form of AB addressing (e.g. F8:0)
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="numberOfElements"></param>
    ''' <param name="dataToWrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteData(ByVal startAddress As String, ByVal numberOfElements As Integer, ByVal dataToWrite() As Single) As Integer
        Dim ParsedResult As ParsedDataAddress = ParseAddress(startAddress)

        Dim ConvertedData(numberOfElements * ParsedResult.BytesPerElements) As Byte

        Dim i As Integer
        If ParsedResult.FileType = &H8A Then
            '*Write to a floating point file
            Dim bytes(4) As Byte
            For i = 0 To numberOfElements - 1
                bytes = BitConverter.GetBytes(CSng(dataToWrite(i)))
                For j As Integer = 0 To 3
                    ConvertedData(i * 4 + j) = CByte(bytes(j))
                Next
            Next
        ElseIf ParsedResult.FileType = &H91 Then
            '* Write to a Long integer file
            While i < numberOfElements
                '* Validate range
                If dataToWrite(i) > 2147483647 Or dataToWrite(i) < -2147483648 Then
                    Throw New DF1Exception("Integer data out of range, must be between -2147483648 and 2147483647")
                End If

                Dim b(3) As Byte
                b = BitConverter.GetBytes(CInt(dataToWrite(i)))

                ConvertedData(i * 4) = b(0)
                ConvertedData(i * 4 + 1) = b(1)
                ConvertedData(i * 4 + 2) = b(2)
                ConvertedData(i * 4 + 3) = b(3)
                i += 1
            End While
        Else
            '* Write to an integer file
            While i < numberOfElements
                '* Validate range
                If dataToWrite(i) > 32767 Or dataToWrite(i) < -32768 Then
                    Throw New DF1Exception("Integer data out of range, must be between -32768 and 32767")
                End If

                ConvertedData(i * 2) = CByte(dataToWrite(i) And &HFF)
                ConvertedData(i * 2 + 1) = CByte((dataToWrite(i) >> 8) And &HFF)
                i += 1
            End While
        End If

        Return WriteRawData(ParsedResult, numberOfElements * ParsedResult.BytesPerElements, ConvertedData)
    End Function

    '* Write a String
    ''' <summary>
    ''' Write a string value to a string data table
    ''' The startAddress is in the common form of AB addressing (e.g. ST9:0)
    ''' </summary>
    ''' <param name="startAddress"></param>
    ''' <param name="dataToWrite"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WriteData(ByVal startAddress As String, ByVal dataToWrite As String) As Integer
        If dataToWrite Is Nothing Then
            Return 0
        End If

        Dim ParsedResult As ParsedDataAddress = ParseAddress(startAddress)

        '* Add an extra character to compensate for characters written in pairs to integers
        Dim ConvertedData(dataToWrite.Length + 2 + 1) As Byte
        dataToWrite &= CStr(0)

        ConvertedData(0) = dataToWrite.Length - 1
        Dim i As Integer = 2
        While i <= dataToWrite.Length
            ConvertedData(i + 1) = CByte(dataToWrite.Substring(i - 2, 1))
            ConvertedData(i) = CByte(dataToWrite.Substring(i - 1, 1))
            i += 2
        End While
        'Array.Copy(System.Text.Encoding.Default.GetBytes(dataToWrite), 0, ConvertedData, 2, dataToWrite.Length)

        Return WriteRawData(ParsedResult, dataToWrite.Length + 2, ConvertedData)
    End Function

    '**************************************************************
    '* Write to a PLC data file
    '*
    '**************************************************************
    Private Function WriteRawData(ByVal ParsedResult As ParsedDataAddress, ByVal numberOfBytes As Integer, ByVal dataToWrite() As Byte) As Integer
        'Dim dataC As New System.Collections.ObjectModel.Collection(Of Byte)

        '* Invalid address?
        If ParsedResult.FileType = 0 Then
            Return -5
        End If

        '**********************************************
        '* Use a bit level function if it is bit level
        '**********************************************
        Dim FunctionNumber As Byte

        Dim FilePosition, NumberOfBytesToWrite, DataStartPosition As Integer

        Dim reply As Integer
        Dim rTNS As Integer

        Do While FilePosition < numberOfBytes AndAlso reply = 0
            '* Set next length of data to read. Max of 236 (slc 5/03 and up)
            '* This must limit to 82 for 5/02 and below
            If numberOfBytes - FilePosition < 164 Then
                NumberOfBytesToWrite = numberOfBytes - FilePosition
            Else
                NumberOfBytesToWrite = 164
            End If

            '* These files seem to be a special case
            If ParsedResult.FileType >= &HA1 And NumberOfBytesToWrite > &H78 Then
                NumberOfBytesToWrite = &H78
            End If

            Dim DataSize As Integer = NumberOfBytesToWrite + 4

            '* For now we are only going to allow one bit to be set/reset per call
            If ParsedResult.BitNumber < 16 Then DataSize = 8

            If ParsedResult.Element >= 255 Then DataSize += 2
            If ParsedResult.SubElement >= 255 Then DataSize += 2

            Dim DataW(DataSize) As Byte

            '* Byte Size
            DataW(0) = ((NumberOfBytesToWrite And &HFF))
            '* File Number
            DataW(1) = (ParsedResult.FileNumber)
            '* File Type
            DataW(2) = (ParsedResult.FileType)
            '* Starting Element Number
            If ParsedResult.Element < 255 Then
                DataW(3) = (ParsedResult.Element)
            Else
                DataW(5) = Math.Floor(ParsedResult.Element / 256)
                DataW(4) = ParsedResult.Element - (DataW(5) * 256) '*  calculate offset
                DataW(3) = 255
            End If

            '* Sub Element
            If ParsedResult.SubElement < 255 Then
                DataW(DataW.Length - 1 - NumberOfBytesToWrite) = ParsedResult.SubElement
            Else
                '* Use extended addressing
                DataW(DataW.Length - 1 - NumberOfBytesToWrite) = Math.Floor(ParsedResult.SubElement / 256)  '* 256+data(5)
                DataW(DataW.Length - 2 - NumberOfBytesToWrite) = ParsedResult.SubElement - (DataW(DataW.Length - 1 - NumberOfBytesToWrite) * 256) '*  calculate offset
                DataW(DataW.Length - 3 - NumberOfBytesToWrite) = 255
            End If

            '* Are we changing a single bit?
            If ParsedResult.BitNumber < 16 Then
                FunctionNumber = &HAB  '* Ref http://www.iatips.com/pccc_tips.html#slc5_cmds
                '* Set the mask of which bit to change
                DataW(DataW.Length - 4) = ((2 ^ (ParsedResult.BitNumber)) And &HFF)
                DataW(DataW.Length - 3) = (2 ^ (ParsedResult.BitNumber - 8))

                If dataToWrite(0) <= 0 Then
                    '* Set bits to clear 
                    DataW(DataW.Length - 2) = 0
                    DataW(DataW.Length - 1) = 0
                Else
                    '* Bits to turn on
                    DataW(DataW.Length - 2) = ((2 ^ (ParsedResult.BitNumber)) And &HFF)
                    DataW(DataW.Length - 1) = (2 ^ (ParsedResult.BitNumber - 8))
                End If
            Else
                FunctionNumber = &HAA
                DataStartPosition = DataW.Length - NumberOfBytesToWrite

                '* Prevent index out of range when numberToWrite exceeds dataToWrite.Length
                Dim ValuesToMove As Integer = NumberOfBytesToWrite - 1
                If ValuesToMove + FilePosition > dataToWrite.Length - 1 Then
                    ValuesToMove = dataToWrite.Length - 1 - FilePosition
                End If

                For i As Integer = 0 To ValuesToMove
                    DataW(i + DataStartPosition) = dataToWrite(i + FilePosition)
                Next
            End If

            reply = PrefixAndSend(&HF, FunctionNumber, DataW, Not m_AsyncMode, rTNS)

            FilePosition += NumberOfBytesToWrite

            If ParsedResult.FileType <> &HA4 Then
                '* Use subelement because it works with all data file types
                ParsedResult.SubElement += NumberOfBytesToWrite / 2
            Else
                '* Special case file - 28h bytes per elements
                ParsedResult.Element += NumberOfBytesToWrite / &H28
            End If
        Loop

        If reply = 0 Then
            Return 0
        Else
            Throw New DF1Exception(DecodeMessage(reply))
        End If
    End Function
    'End of Public Methods
#End Region

#Region "Shared Methods"
    '****************************************************************
    '* Convert an array of words into a string as AB PLC's represent
    '* Can be used when reading a string from an Integer file
    '****************************************************************
    ''' <summary>
    ''' Convert an array of integers to a string
    ''' This is used when storing strings in an integer data table
    ''' </summary>
    ''' <param name="words"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WordsToString(ByVal words() As Int32) As String
        Dim WordCount As Integer = words.Length
        Return WordsToString(words, 0, WordCount)
    End Function

    ''' <summary>
    ''' Convert an array of integers to a string
    ''' This is used when storing strings in an integer data table
    ''' </summary>
    ''' <param name="words"></param>
    ''' <param name="index"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WordsToString(ByVal words() As Int32, ByVal index As Integer) As String
        Dim WordCount As Integer = (words.Length - index)
        Return WordsToString(words, index, WordCount)
    End Function

    ''' <summary>
    ''' Convert an array of integers to a string
    ''' This is used when storing strings in an integer data table
    ''' </summary>
    ''' <param name="words"></param>
    ''' <param name="index"></param>
    ''' <param name="wordCount"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function WordsToString(ByVal words() As Int32, ByVal index As Integer, ByVal wordCount As Integer) As String
        Dim j As Integer = index
        Dim result2 As New System.Text.StringBuilder
        While j < wordCount
            result2.Append(CStr(words(j) / 256))
            '* Prevent an odd length string from getting a Null added on
            If CInt(words(j) And &HFF) > 0 Then
                result2.Append(CStr(words(j) And &HFF))
            End If
            j += 1
        End While

        Return result2.ToString
    End Function


    '**********************************************************
    '* Convert a string to an array of words
    '*  Can be used when writing a string to an Integer file
    '**********************************************************
    ''' <summary>
    ''' Convert a string to an array of words
    ''' Can be used when writing a string into an integer data table
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function StringToWords(ByVal source As String) As Int32()
        If source Is Nothing Then
            Return Nothing
            ' Throw New ArgumentNullException("input")
        End If

        Dim ArraySize As Integer = CInt(Math.Ceiling(source.Length / 2)) - 1

        Dim ConvertedData(ArraySize) As Int32

        Dim i As Integer
        While i <= ArraySize
            ConvertedData(i) = CByte(source.Substring(i * 2, 1)) * 256
            '* Check if past last character of odd length string
            If (i * 2) + 1 < source.Length Then ConvertedData(i) += CByte(source.Substring((i * 2) + 1, 1))
            i += 1
        End While

        Return ConvertedData
    End Function

#End Region

#Region "Helper"
    Private Structure ParsedDataAddress
        Dim FileType As Integer
        Dim FileNumber As Integer
        Dim Element As Integer
        Dim SubElement As Integer
        Dim BitNumber As Integer
        Dim BytesPerElements As Integer
        Dim TableSizeInBytes As Integer
    End Structure

    '*********************************************************************************
    '* Parse the address string and validate, if invalid, Return 0 in FileType
    '* Convert the file type letter Type to the corresponding value
    '* Reference page 7-18
    '*********************************************************************************
    Private RE1 As New Regex("(?i)^\s*(?<FileType>([SBCTRNFAIOL])|(ST)|(MG)|(PD)|(PLS))(?<FileNumber>\d{1,3}):(?<ElementNumber>\d{1,3})(/(?<BitNumber>\d{1,4}))?\s*$")
    Private RE2 As New Regex("(?i)^\s*(?<FileType>[BN])(?<FileNumber>\d{1,3})(/(?<BitNumber>\d{1,4}))\s*$")
    Private RE3 As New Regex("(?i)^\s*(?<FileType>[CT])(?<FileNumber>\d{1,3}):(?<ElementNumber>\d{1,3})[.](?<SubElement>(ACC|PRE|EN|DN|TT|CU|CD|DN|OV|UN|UA))\s*$")
    '* IO variation without file number Type (Input : file 1, Output : file 0 )
    Private RE4 As New Regex("(?i)^\s*(?<FileType>([IOS])):(?<ElementNumber>\d{1,3})([.](?<SubElement>[0-7]))?(/(?<BitNumber>\d{1,4}))?\s*$")
    Private Function ParseAddress(ByVal DataAddress As String) As ParsedDataAddress
        Dim result As ParsedDataAddress

        result.FileType = 0  '* Let a 0 inidcated an invalid address
        result.BitNumber = 99  '* Let a 99 indicate no bit level requested

        '*********************************
        '* Try all match patterns
        '*********************************
        Dim mc As MatchCollection = RE1.Matches(DataAddress)

        If mc.Count <= 0 Then
            mc = RE2.Matches(DataAddress)
            If mc.Count <= 0 Then
                mc = RE3.Matches(DataAddress)
                If mc.Count <= 0 Then
                    mc = RE4.Matches(DataAddress)
                    If mc.Count <= 0 Then
                        Return result
                    End If
                End If
            End If
        End If

        '*********************************************
        '* Get elements extracted from match patterns
        '*********************************************
        '* Is it an I,O, or S address without a file number Type?
        If mc.Item(0).Groups("FileNumber").Length = 0 Then
            ' Is it an input or Output file?
            If DataAddress.IndexOf("i") >= 0 Or DataAddress.IndexOf("I") >= 0 Then
                result.FileNumber = 1
            ElseIf DataAddress.IndexOf("o") >= 0 Or DataAddress.IndexOf("O") >= 0 Then
                result.FileNumber = 0
            Else
                result.FileNumber = 2
            End If
        Else
            result.FileNumber = mc.Item(0).Groups("FileNumber").ToString
        End If


        If mc.Item(0).Groups("BitNumber").Length > 0 Then
            result.BitNumber = mc.Item(0).Groups("BitNumber").ToString
        End If

        If mc.Item(0).Groups("ElementNumber").Length > 0 Then
            result.Element = mc.Item(0).Groups("ElementNumber").ToString
        Else
            result.Element = result.BitNumber >> 4
            result.BitNumber = result.BitNumber Mod 16
        End If

        If mc.Item(0).Groups("SubElement").Length > 0 Then
            Select Case mc.Item(0).Groups("SubElement").ToString.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
                Case "PRE" : result.SubElement = 1
                Case "ACC" : result.SubElement = 2
                Case "EN" : result.SubElement = 15
                Case "TT" : result.SubElement = 14
                Case "DN" : result.SubElement = 13
                Case "CU" : result.SubElement = 15
                Case "CD" : result.SubElement = 14
                Case "OV" : result.SubElement = 12
                Case "UN" : result.SubElement = 11
                Case "UA" : result.SubElement = 10
                Case "0" : result.SubElement = 0
                Case "1" : result.SubElement = 1
                Case "2" : result.SubElement = 2
                Case "3" : result.SubElement = 3
                Case "4" : result.SubElement = 4
                Case "5" : result.SubElement = 5
                Case "6" : result.SubElement = 6
                Case "7" : result.SubElement = 7
                Case "8" : result.SubElement = 8
            End Select
        End If


        '* These subelements are bit level
        If result.SubElement > 4 Then
            result.SubElement = 0
            result.BitNumber = result.SubElement
        End If


        '***************************************
        '* Translate file type letter to number
        '***************************************
        If result.Element < 256 Then
            result.BytesPerElements = 2

            Dim FileType As String = mc.Item(0).Groups("FileType").ToString.ToUpper(System.Globalization.CultureInfo.CurrentCulture)
            Select Case FileType
                Case "N" : result.FileType = &H89
                Case "B" : result.FileType = &H85
                Case "T" : result.FileType = &H86
                Case "C" : result.FileType = &H87
                Case "F" : result.FileType = &H8A
                    result.BytesPerElements = 4
                Case "S" : result.FileType = &H84
                Case "ST" : result.FileType = &H8D
                    result.BytesPerElements = 76
                Case "A" : result.FileType = &H8E
                Case "R" : result.FileType = &H88
                Case "O" : result.FileType = &H8B
                Case "I" : result.FileType = &H8C
                Case "L" : result.FileType = &H91
                    result.BytesPerElements = 4
                Case "MG" : result.FileType = &H92   'Message Command 146
                    result.BytesPerElements = 50
                Case "PD" : result.FileType = &H93   'PID
                    result.BytesPerElements = 46
                Case "PLS" : result.FileType = &H94   'Programmable Limit Swith
                    result.BytesPerElements = 12
            End Select
        End If

        Return result
    End Function

    '****************************************************
    '* Wait for a response from PLC before returning
    '****************************************************
    Dim MaxTicks As Integer = 100  '* 50 ticks per second
    Private Function WaitForResponse(ByVal rTNS As Integer) As Integer
        'Responded = False

        Dim Loops As Integer = 0
        While Not Responded(rTNS) And Loops < MaxTicks
            'Application.DoEvents()
            System.Threading.Thread.Sleep(20)
            Loops += 1
        End While

        If Loops >= MaxTicks Then
            Return -20
        ElseIf LastResponseWasNAK Then
            Return -21
        Else
            Return 0
        End If
    End Function

    '**************************************************************
    '* This method implements the common application routine
    '* as discussed in the Software Layer section of the AB manual
    '**************************************************************
    Private Function PrefixAndSend(ByVal Command As Byte, ByVal Func As Byte, ByVal data() As Byte, ByVal Wait As Boolean, ByRef rTNS As Integer) As Integer
        'Dim CommandPacket As New System.Collections.ObjectModel.Collection(Of Byte)

        IncrementTNS()


        Dim PacketSize As Integer
        If m_Protocol = "DF1" Then
            PacketSize = data.Length + 6
        Else
            PacketSize = data.Length + 10
        End If


        Dim CommandPacke(PacketSize) As Byte
        Dim BytePos As Integer

        If m_Protocol = "DF1" Then
            CommandPacke(0) = m_TargetNode
            CommandPacke(1) = m_MyNode
            BytePos = 2
        Else
            CommandPacke(0) = m_TargetNode + &H80
            CommandPacke(1) = &H88                  '* Not sure what this is, must be command code for DF1 packet. Sometimes it's &H18
            CommandPacke(2) = m_MyNode + &H80
            CommandPacke(3) = 1
            CommandPacke(4) = 1
            CommandPacke(5) = data.Length + 5    '*Length of DF1 data packet
            BytePos = 6
        End If

        CommandPacke(BytePos) = Command
        CommandPacke(BytePos + 1) = 0       '* STS (status, always 0)

        CommandPacke(BytePos + 2) = (TNS And 255)
        CommandPacke(BytePos + 3) = (TNS >> 8)

        CommandPacke(BytePos + 4) = Func

        data.CopyTo(CommandPacke, BytePos + 5)

        rTNS = TNS And &HFF
        Responded(rTNS) = False
        Dim result As Integer
        If m_Protocol = "DF1" Then
            ACKed(TNS And 255) = False
            result = SendData(CommandPacke)
        Else
            If Not SerialPort.IsOpen Then OpenComms()
            QueuedCommand.Clear()
            For j As Integer = 0 To CommandPacke.Length - 1
                QueuedCommand.Add(CommandPacke(j))
            Next
            'Array.Copy(CommandPacke, QueuedCommand, CommandPacke.Length)
            'QueuedCommandSize = CommandPacke.Length
            CommandInQueue = True
        End If


        If result = 0 And Wait Then
            result = WaitForResponse(rTNS)

            '* Return status byte that came from controller
            If result = 0 Then
                If DataPackets(rTNS) IsNot Nothing Then
                    If m_Protocol = "DF1" Then
                        If (DataPackets(rTNS).Count > 3) Then
                            result = DataPackets(rTNS)(3)  '* STS position in DF1 message
                            '* If its and EXT STS, page 8-4
                            If result = &HF0 Then
                                '* The EXT STS is the last byte in the packet
                                'result = DataPackets(rTNS)(DataPackets(rTNS).Count - 2) + &H100
                                result = DataPackets(rTNS)(DataPackets(rTNS).Count - 1) + &H100
                            End If
                        End If
                    Else
                        If DataPackets(rTNS).Count > 7 Then '* STS position in DH485 message
                            result = DataPackets(rTNS)(7)
                        End If
                    End If
                Else
                    result = -8 '* no response came back from PLC
                End If
            Else
                Dim DebugCheck As Integer = 0
            End If
        Else
            Dim DebugCheck As Integer = 0
        End If

        Return result
    End Function

    '**************************************************************
    '* This method Sends a response from an unsolicited msg
    '**************************************************************
    Private Function SendResponse(ByVal Command As Byte, ByVal rTNS As Integer) As Integer
        Dim PacketSize As Integer
        'PacketSize = Data.Length + 5
        PacketSize = 5


        Dim CommandPacke(PacketSize) As Byte
        Dim BytePos As Integer

        CommandPacke(1) = m_TargetNode
        CommandPacke(0) = m_MyNode
        BytePos = 2

        CommandPacke(BytePos) = Command
        CommandPacke(BytePos + 1) = 0       '* STS (status, always 0)

        CommandPacke(BytePos + 2) = (rTNS And 255)
        CommandPacke(BytePos + 3) = (rTNS >> 8)


        Dim result As Integer
        result = SendData(CommandPacke)
        Return result
    End Function

    Private Sub IncrementTNS()
        '* Incement the TransactionNumber value
        If TNS < 65535 Then
            TNS += 1
        Else
            TNS = 1
        End If
    End Sub

    '************************************************
    '* Conver the message code number into a string
    '* Ref Page 8-3'************************************************
    Public Shared Function DecodeMessage(ByVal msgNumber As Integer) As String
        Select Case msgNumber
            Case 0
                DecodeMessage = ""
            Case -2
                Return "Not Acknowledged (NAK)"
            Case -3
                Return "No Reponse, Check COM Settings"
            Case -4
                Return "Unknown Message from DataLink Layer"
            Case -5
                Return "Invalid Address"
            Case -6
                Return "Could Not Open Com Port"
            Case -7
                Return "No data specified to data link layer"
            Case -8
                Return "No data returned from PLC"
            Case -20
                Return "No Data Returned"
            Case -21
                Return "Received Message NAKd from invalid checksum"

                '*** Errors coming from PLC
            Case 16
                Return "Illegal Command or Format, Address may not exist or not enough elements in data file"
            Case 32
                Return "PLC Has a Problem and Will Not Communicate"
            Case 48
                Return "Remote Node Host is Misssing, Disconnected, or Shut Down"
            Case 64
                Return "Host Could Not Complete Function Due To Hardware Fault"
            Case 80
                Return "Addressing problem or Memory Protect Rungs"
            Case 96
                Return "Function not allows due to command protection selection"
            Case 112
                Return "Processor is in Program mode"
            Case 128
                Return "Compatibility mode file missing or communication zone problem"
            Case 144
                Return "Remote node cannot buffer command"
            Case 240
                Return "Error code in EXT STS Byte"

                '* EXT STS Section - 256 is added to code to distinguish EXT codes
            Case 257
                Return "A field has an illegal value"
            Case 258
                Return "Less levels specified in address than minimum for any address"
            Case 259
                Return "More levels specified in address than system supports"
            Case 260
                Return "Symbol not found"
            Case 261
                Return "Symbol is of improper format"
            Case 262
                Return "Address doesn't point to something usable"
            Case 263
                Return "File is wrong size"
            Case 264
                Return "Cannot complete request, situation has changed since the start of the command"
            Case 265
                Return "Data or file is too large"
            Case 266
                Return "Transaction size plus word address is too large"
            Case 267
                Return "Access denied, improper priviledge"
            Case 268
                Return "Condition cannot be generated - resource is not available"
            Case 269
                Return "Condition already exists - resource is already available"
            Case 270
                Return "Command cannot be executed"

            Case Else
                Return "Unknown Message - " & msgNumber
        End Select
    End Function


    'Private Sub DF1DataLink1_DataReceived(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.DataReceivedDL
    Private Sub DF1DataLink1_DataReceived()
        ' DataString(LastDataIndex) = DataStrings(LastDataIndex)

        '* Should we only raise an event if we are in AsyncMode?
        '        If m_AsyncMode Then
        '**************************************************************************
        '* If the parent form property is set, then sync the event with its thread
        '**************************************************************************
        '* This was moved
        'Responded(m_LastDataIndex) = True

        'RaiseEvent DataReceived(Me, System.EventArgs.Empty)
        'Exit Sub

        If Not DisableEvent Then
            RaiseEvent DataReceived(Me, System.EventArgs.Empty)
        End If
        'End If
    End Sub

    '******************************************************************
    '* This is called when a message instruction was sent from the PLC
    '******************************************************************
    Private Sub DF1DataLink1_UnsolictedMessageRcvd()
        RaiseEvent UnsolictedMessageRcvd(Me, System.EventArgs.Empty)
    End Sub


    '****************************************************************************
    '* This is required to sync the event back to the parent form's main thread
    '****************************************************************************
    Dim drsd As EventHandler = AddressOf DataReceivedSync
    'Delegate Sub DataReceivedSyncDel(ByVal sender As Object, ByVal e As EventArgs)
    Private Sub DataReceivedSync(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent DataReceived(sender, e)
    End Sub
    Private Sub UnsolictedMessageRcvdSync(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent UnsolictedMessageRcvd(sender, e)
    End Sub
#End Region

#Region "Data Link Layer"
    '**************************************************************************************************************
    '**************************************************************************************************************
    '**************************************************************************************************************
    '*****                                  DATA LINK LAYER SECTION
    '**************************************************************************************************************
    '**************************************************************************************************************

    'Private Response As ResponseTypes
    Private DataPackets(255) As System.Collections.ObjectModel.Collection(Of Byte)
    Private LastResponseWasNAK As Boolean

    Private WithEvents SerialPort As New System.IO.Ports.SerialPort

    'Private Enum ResponseTypes
    '    NoResponse
    '    AXcknowledged
    '    NotAXcknowledged
    '    TimeOut
    '    DataReturned
    '    Enquire
    'End Enum

    'Private Event DataReceivedDL(ByVal sender As Object, ByVal e As EventArgs)


    '*********************************************************
    '* This keeps a buffer of the last 256 messages received
    '* Its key is based on the LSB of the TNS value
    '*********************************************************
    'Private m_LastDataIndex As Integer
    'Public ReadOnly Property LastDataIndex() As Integer
    '    Get
    '        Return m_LastDataIndex
    '    End Get
    'End Property

    '*******************************
    '* Table for calculating CRC
    '*******************************
    Private ReadOnly aCRC16Table() As UInt16 = {&H0, &HC0C1, &HC181, &H140, &HC301, &H3C0, &H280, &HC241,
        &HC601, &H6C0, &H780, &HC741, &H500, &HC5C1, &HC481, &H440,
        &HCC01, &HCC0, &HD80, &HCD41, &HF00, &HCFC1, &HCE81, &HE40,
        &HA00, &HCAC1, &HCB81, &HB40, &HC901, &H9C0, &H880, &HC841,
        &HD801, &H18C0, &H1980, &HD941, &H1B00, &HDBC1, &HDA81, &H1A40,
        &H1E00, &HDEC1, &HDF81, &H1F40, &HDD01, &H1DC0, &H1C80, &HDC41,
        &H1400, &HD4C1, &HD581, &H1540, &HD701, &H17C0, &H1680, &HD641,
        &HD201, &H12C0, &H1380, &HD341, &H1100, &HD1C1, &HD081, &H1040,
        &HF001, &H30C0, &H3180, &HF141, &H3300, &HF3C1, &HF281, &H3240,
        &H3600, &HF6C1, &HF781, &H3740, &HF501, &H35C0, &H3480, &HF441,
        &H3C00, &HFCC1, &HFD81, &H3D40, &HFF01, &H3FC0, &H3E80, &HFE41,
        &HFA01, &H3AC0, &H3B80, &HFB41, &H3900, &HF9C1, &HF881, &H3840,
        &H2800, &HE8C1, &HE981, &H2940, &HEB01, &H2BC0, &H2A80, &HEA41,
        &HEE01, &H2EC0, &H2F80, &HEF41, &H2D00, &HEDC1, &HEC81, &H2C40,
        &HE401, &H24C0, &H2580, &HE541, &H2700, &HE7C1, &HE681, &H2640,
        &H2200, &HE2C1, &HE381, &H2340, &HE101, &H21C0, &H2080, &HE041,
        &HA001, &H60C0, &H6180, &HA141, &H6300, &HA3C1, &HA281, &H6240,
        &H6600, &HA6C1, &HA781, &H6740, &HA501, &H65C0, &H6480, &HA441,
        &H6C00, &HACC1, &HAD81, &H6D40, &HAF01, &H6FC0, &H6E80, &HAE41,
        &HAA01, &H6AC0, &H6B80, &HAB41, &H6900, &HA9C1, &HA881, &H6840,
        &H7800, &HB8C1, &HB981, &H7940, &HBB01, &H7BC0, &H7A80, &HBA41,
        &HBE01, &H7EC0, &H7F80, &HBF41, &H7D00, &HBDC1, &HBC81, &H7C40,
        &HB401, &H74C0, &H7580, &HB541, &H7700, &HB7C1, &HB681, &H7640,
        &H7200, &HB2C1, &HB381, &H7340, &HB101, &H71C0, &H7080, &HB041,
        &H5000, &H90C1, &H9181, &H5140, &H9301, &H53C0, &H5280, &H9241,
        &H9601, &H56C0, &H5780, &H9741, &H5500, &H95C1, &H9481, &H5440,
        &H9C01, &H5CC0, &H5D80, &H9D41, &H5F00, &H9FC1, &H9E81, &H5E40,
        &H5A00, &H9AC1, &H9B81, &H5B40, &H9901, &H59C0, &H5880, &H9841,
        &H8801, &H48C0, &H4980, &H8941, &H4B00, &H8BC1, &H8A81, &H4A40,
        &H4E00, &H8EC1, &H8F81, &H4F40, &H8D01, &H4DC0, &H4C80, &H8C41,
        &H4400, &H84C1, &H8581, &H4540, &H8701, &H47C0, &H4680, &H8641,
        &H8201, &H42C0, &H4380, &H8341, &H4100, &H81C1, &H8081, &H4040}

    '*********************************
    '* This CRC uses a table lookup
    '* algorithm for faster computing
    '*********************************
    Private Function CalculateCRC16(ByVal DataInput() As Byte) As Integer
        Dim iCRC As UInt16 = 0
        Dim bytT As Byte


        For i As Integer = 0 To DataInput.Length - 1
            bytT = (iCRC And &HFF) Xor DataInput(i)
            iCRC = (iCRC >> 8) Xor aCRC16Table(bytT)
        Next

        '*** must do one more with ETX char
        bytT = (iCRC And &HFF) Xor 3
        iCRC = (iCRC >> 8) Xor aCRC16Table(bytT)

        Return iCRC
    End Function

    '* Overload - Calc CRC on a collection of bytes
    Private Function CalculateCRC16(ByVal DataInput As System.Collections.ObjectModel.Collection(Of Byte)) As Integer
        Dim iCRC As UInt16 = 0
        Dim bytT As Byte


        For i As Integer = 0 To DataInput.Count - 1
            bytT = (iCRC And &HFF) Xor DataInput(i)
            iCRC = (iCRC >> 8) Xor aCRC16Table(bytT)
        Next

        '*** must do one more with ETX char
        bytT = (iCRC And &HFF) Xor 3
        iCRC = (iCRC >> 8) Xor aCRC16Table(bytT)

        Return iCRC
    End Function

    '***************************
    '* Calculate a BCC
    '***************************
    Private Shared Function CalculateBCC(ByVal DataInput() As Byte) As Byte
        Dim sum As Integer = 0
        For i As Integer = 0 To DataInput.Length - 1
            sum += DataInput(i)
        Next

        CalculateBCC = sum And &HFF
        CalculateBCC = (&H100 - CalculateBCC) And 255    '* had to add the "and 255" for the case of sum being 0
    End Function
    '* Overload
    Private Shared Function CalculateBCC(ByVal DataInput As System.Collections.ObjectModel.Collection(Of Byte)) As Byte
        Dim sum As Integer = 0
        For i As Integer = 0 To DataInput.Count - 1
            sum += DataInput(i)
        Next

        CalculateBCC = sum And &HFF
        CalculateBCC = (&H100 - CalculateBCC) And 255    '* had to add the "and 255" for the case of sum being 0
    End Function

    '******************************************
    '* Handle Data Received On The Serial Port
    '******************************************
    Private LastByte As Byte
    Private PacketStarted As Boolean
    Private PacketEnded As Boolean
    Private NodeChecked As Boolean
    Dim b As Byte
    Private ETXPosition As Integer
    Private ReceivedDataPacket As New System.Collections.ObjectModel.Collection(Of Byte)
    Private Sub SerialPort_DataReceived(BytesRead As Byte())
        Dim BytesToRead As Integer = BytesRead.Length

        Dim i As Integer = 0

        While i < BytesToRead
            b = BytesRead(i)

            '**************************************************************
            '* Do not start capturing chars until start of packet received
            '**************************************************************
            If PacketStarted Then
                '* filter out double 16's
                If LastByte = 16 And b = 16 Then
                    b = 0
                    LastByte = 0
                Else
                    ReceivedDataPacket.Add(b)
                End If

                '* Is there another start sequence?
                If LastByte = 16 And b = 2 Then
                    ReceivedDataPacket.Clear()
                End If

                '* Ignore data if not addressed to this node
                If Not NodeChecked Then
                    If m_Protocol <> "DF1" And b <> m_MyNode + &H80 Then
                        PacketStarted = False
                        PacketEnded = False
                        ReceivedDataPacket.Clear()
                    Else
                        NodeChecked = True
                        SerialPort.DtrEnable = False
                    End If
                End If
            End If


            '******************
            '* DLE character
            '******************
            If LastByte = 16 Then
                '******************
                '* STX Sequence
                '******************
                If b = 2 Then
                    PacketStarted = True
                    NodeChecked = False
                End If

                '*******************
                '* ETX Sequence
                '*******************
                If PacketStarted AndAlso b = 3 Then
                    PacketEnded = True
                    ETXPosition = ReceivedDataPacket.Count - 2
                End If

                '********************************
                '* Handle DF1 Control Characters
                '********************************
                If m_Protocol = "DF1" AndAlso b <> 2 AndAlso b <> 16 AndAlso b <> 3 Then
                    '***************
                    '* ACK sequence
                    '***************
                    If b = 6 Then
                        System.Threading.Thread.Sleep(SleepDelay)
                        Acknowledged = True
                        ACKed(TNS And 255) = True
                    End If

                    '***************
                    '* NAK Sequence
                    '***************
                    If b = 21 Then
                        NotAcknowledged = True
                    End If

                    '***************
                    '* ENQ Sequence
                    '***************
                    If b = 5 Then
                        '* We can handle the ENQ right here
                        'Response = ResponseTypes.Enquire

                        '* Reply with last response back to PLC
                        Dim ACKSequence() As Byte = {16, 6}
                        If LastResponseWasNAK Then
                            ACKSequence(1) = &H15 '* NAK
                        End If
                        SerialPort.Write(ACKSequence, 0, 2)
                    End If

                    '* Removed this because it cleared out good data when a "16" byte came
                    'ReceivedDataPacket.Clear()
                End If
            End If



            If PacketEnded Then
                If (m_CheckSum = CheckSumOptions.Bcc And (ReceivedDataPacket.Count - ETXPosition) >= 3) Or (m_CheckSum = CheckSumOptions.Crc And (ReceivedDataPacket.Count - ETXPosition) >= 4) Then
                    ProcessReceivedData()
                    PacketStarted = False
                    PacketEnded = False
                    b = 0 'make sure last byte isn't falsely 16
                    ReceivedDataPacket.Clear()
                End If
            End If


            LastByte = b
            i += 1
        End While
    End Sub

    '*********************************
    '* Created for auto configure
    '*********************************
    Private Function SendENQ() As Integer
        If Not SerialPort.IsOpen Then
            Dim OpenResult As Integer = OpenComms()
            If OpenResult <> 0 Then Return OpenResult
        End If

        Dim ENQSequence() As Byte = {16, 5}
        Acknowledged = False
        NotAcknowledged = False
        SerialPort.Write(ENQSequence, 0, 2)

        AckWaitTicks = 0
        While (Not Acknowledged And Not NotAcknowledged) And AckWaitTicks < MaxTicks
            System.Threading.Thread.Sleep(20)
            AckWaitTicks += 1
        End While

        If AckWaitTicks >= MaxTicks Then Return -3

        Return 0
    End Function


    Private PacketOpened As Boolean
    '*******************************************************************
    '* When a complete packet is received from the PLC, this is called
    '*******************************************************************
    Private Sub ProcessReceivedData()
        '* Get the Checksum that came back from the PLC
        Dim CheckSumResult As UInt16
        If m_CheckSum = CheckSumOptions.Bcc Then
            CheckSumResult = ReceivedDataPacket(ETXPosition + 2)
        Else
            CheckSumResult = (ReceivedDataPacket(ETXPosition + 2)) + (ReceivedDataPacket(ETXPosition + 3)) * 256
        End If


        '****************************
        '* validate CRC received
        '****************************
        '* Store the returned data in an array based on the LSB of the TNS
        '* If there is no TNS, then store in 0
        Dim xTNS As Integer

        '* make sure there is enough data and it is not a command (commands are less than 31)
        If ETXPosition > 4 And ReceivedDataPacket(2) > 31 Then
            xTNS = ReceivedDataPacket(4)
        Else
            xTNS = 0
        End If

        If m_Protocol <> "DF1" AndAlso ETXPosition > 8 Then
            xTNS = ReceivedDataPacket(8)
        End If

        '********************************************************************
        '* Store data in a array of collection using TNS's low byte as index
        '********************************************************************
        If DataPackets(xTNS) IsNot Nothing Then
            DataPackets(xTNS).Clear()
        Else
            DataPackets(xTNS) = New System.Collections.ObjectModel.Collection(Of Byte)
        End If

        For i As Integer = 0 To ETXPosition - 1
            DataPackets(xTNS).Add(ReceivedDataPacket(i))
        Next


        '* Calculate the checksum for the received data
        Dim CheckSumCalc As Integer
        If m_CheckSum = CheckSumOptions.Bcc Then
            CheckSumCalc = CalculateBCC(DataPackets(xTNS))
        Else
            CheckSumCalc = CalculateCRC16(DataPackets(xTNS))
        End If


        '***************************************************************************
        '* Send back an response to indicate whether data was received successfully
        '***************************************************************************
        Dim ACKSequence() As Byte = {16, 6}
        'CheckSumCalc = 0 '*** DEBUG - fail checksum
        If CheckSumResult = CheckSumCalc Then
            If m_Protocol = "DF1" Then
                Responded(xTNS) = True

                If DataPackets(xTNS)(2) > 31 Then
                    '* Let application layer know that new data has came back
                    DF1DataLink1_DataReceived()
                Else
                    '****************************************************
                    '****************************************************
                    '* Handle the unsolicited message
                    '* This is where the simulator code would be placed
                    '****************************************************
                    '* Command &h0F Function &HAA - Logical Write
                    If DataPackets(xTNS)(2) = 15 And DataPackets(xTNS)(6) = &HAA Then
                        '* Send back response - Page 7-18
                        Dim TNS As Integer
                        TNS = DataPackets(xTNS)(5) * 256 + DataPackets(xTNS)(4)
                        SendResponse(DataPackets(xTNS)(2) + &H40, TNS)

                        '* Extract the information
                        Dim ElementCount As Integer = DataPackets(xTNS)(7)
                        Dim FileNumber As Integer = DataPackets(xTNS)(8)
                        Dim FileType As Integer = DataPackets(xTNS)(9)
                        Dim Element As Integer = DataPackets(xTNS)(10)
                        Dim SubElement As Integer = DataPackets(xTNS)(11)
                        Dim StringFileType As String
                        Dim BytesPerElement As Integer

                        Select Case FileType
                            Case &H89 : StringFileType = "N"
                                BytesPerElement = 2
                            Case &H85 : StringFileType = "B"
                                BytesPerElement = 2
                            Case &H86 : StringFileType = "T"
                                BytesPerElement = 6
                            Case &H87 : StringFileType = "C"
                                BytesPerElement = 6
                            Case &H84 : StringFileType = "S"
                                BytesPerElement = 2
                            Case &H8A : StringFileType = "F"
                                BytesPerElement = 4
                            Case &H8D : StringFileType = "ST"
                                BytesPerElement = 84
                            Case &H8E : StringFileType = "A"
                                BytesPerElement = 2
                            Case &H88 : StringFileType = "R"
                                BytesPerElement = 6
                            Case &H82, &H8B : StringFileType = "O"
                                BytesPerElement = 2
                            Case &H83, &H8C : StringFileType = "I"
                                BytesPerElement = 2

                            Case Else : StringFileType = "Undefined"
                                BytesPerElement = 2
                        End Select


                        '* Raise the event to let know that a command was rcvd
                        DF1DataLink1_UnsolictedMessageRcvd()
                    End If
                    Exit Sub
                End If
            End If

            '* Keep this in case the PLC requests with ENQ
            LastResponseWasNAK = False
        Else
            ACKSequence(1) = &H15 '* NAK
            AckWaitTicks = 0

            '* Keep this in case the PLC requests with ENQ
            Responded(xTNS) = True
            LastResponseWasNAK = True

            '* Slow down comms - helps with USB converter
            If SleepDelay < 400 Then SleepDelay += 50
        End If

        '*********************************
        '* Respond according to protocol
        If m_Protocol = "DF1" Then
            '* Send the ACK or NAK back to the PLC
            SerialPort.Write(ACKSequence, 0, 2)
        Else
            '*********************************************
            '* DH485 command responses
            '**********************************************
            '* Ack command received from DH485
            If ReceivedDataPacket(1) = &H18 Then
                Dim aa() As Byte = {m_TargetNode + &H80, 0, m_MyNode + &H80}
                IncrementTNS()
                SendData(aa)
                PacketOpened = True
                CommandInQueue = False '* Do not clear command until it is acknowledged, this forces continuous retries
            End If

            '* Send back an Acknowledge of data received
            If ReceivedDataPacket(1) > 0 And ReceivedDataPacket(1) <> &H18 Then
                Dim a() As Byte = {m_TargetNode + &H80, &H18, m_MyNode + &H80}
                IncrementTNS()
                SendData(a)
                PacketOpened = True

                If ReceivedDataPacket(1) > 1 Then
                    If (ReceivedDataPacket(1) And 31) = &H8 Then
                        'Response = ResponseTypes.DataReturned
                        DF1DataLink1_DataReceived()
                    End If
                End If
            End If
            '* Token Passed to node
            If ReceivedDataPacket(1) = 0 Then
                If Not CommandInQueue Or PacketOpened Then
                    Dim a() As Byte = {m_TargetNode + &H80, 0, m_MyNode + &H80}
                    SendData(a)
                    PacketOpened = False
                Else
                    Dim QC(QueuedCommand.Count - 1) As Byte
                    QueuedCommand.CopyTo(QC, 0)
                    SendData(QC)
                    'CommandInQueue = False '* Do not clear command until it is acknowledged, this forces continuous retries
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Opens the comm port to start communications
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function OpenComms()
        '*****************************************
        '* Open serial port if not already opened
        '*****************************************
        If Not SerialPort.IsOpen Then
            SerialPort.BaudRate = m_BaudRate
            SerialPort.PortName = m_ComPort
            SerialPort.Parity = m_Parity
            'SerialPort.Handshake = IO.Ports.Handshake.RequestToSend
            'SerialPort.ReadBufferSize = 16384
            'SerialPort.ReceivedBytesThreshold = 1
            'SerialPort.WriteBufferSize = 2048
            Dim blocklimit As Integer = 16384
            Dim readBuffer As Byte() = New Byte(blocklimit - 1) {}
            Dim kickoffRead As Action(Of Object) = Nothing

            '            Byte[] buffer = New Byte[blocklimit];
            'Action kickoffRead = null;
            'kickoffRead = delegate {
            '    port.BaseStream.BeginRead(Buffer, 0, Buffer.Length, delegate (IAsyncResult ar) {
            '    Try {
            '        int actualLenght = port.BaseStream.EndRead(ar);
            'Byte[] received = New Byte[actualLength];
            'Buffer.BlockCopy(Buffer, 0, received, 0, actualLength);
            '       raiseAppSerialDataEvent(received);
            '    }
            '    Catch (IOException exc) {
            '        handleAppSerialError(exc);
            '    }
            '    kickoffRead();
            '    }, null);
            '};
            'kickoffRead();
            kickoffRead = Sub(obj As Object) SerialPort.BaseStream.BeginRead(readBuffer, 0, readBuffer.Length,
                Sub(ar As IAsyncResult)
                    Try
                        Dim actualLength As Integer = 1
                        If SerialPort.IsOpen Then
                            Try
                                actualLength = SerialPort.BaseStream.EndRead(ar)
                            Catch exc As System.InvalidOperationException
                                Debug.WriteLine("Serial DF1 Error: " & exc.Message)
                                If SerialPort.IsOpen Then
                                    SerialPort.DiscardInBuffer()
                                    CloseComms()
                                End If
                                Return
                            Catch exc As System.ArgumentException
                                Debug.WriteLine("Serial DF1 Error: " & exc.Message)
                                If SerialPort.IsOpen Then
                                    SerialPort.DiscardInBuffer()
                                    CloseComms()
                                End If
                                Return
                            End Try
                            Dim received As Byte() = New Byte(actualLength - 1) {}
                            Buffer.BlockCopy(readBuffer, 0, received, 0, actualLength)
                            SerialPort_DataReceived(received)
                        End If
                    Catch exc As System.IO.IOException
                        If SerialPort.IsOpen Then SerialPort.DiscardInBuffer()
                        'Throw New DF1Exception("Failed To Read " & SerialPort.PortName & ". " & exc.Message)
                        'Catch exc As System.InvalidOperationException
                        '    Throw New DF1Exception(SerialPort.PortName & " is not open. " & exc.Message)
                        Debug.WriteLine("Serial DF1 Error: " & exc.Message)
                    End Try
                    If SerialPort.IsOpen Then kickoffRead(Nothing)

                End Sub, Nothing)

            Try
                SerialPort.Open()
                SerialPort.DiscardInBuffer()
                kickoffRead(Nothing)
                If Protocol <> "DF1" Then
                    SerialPort.DtrEnable = True
                    SerialPort.RtsEnable = False
                End If
            Catch ex As Exception
                Throw New DF1Exception("Failed To Open " & SerialPort.PortName & ". " & ex.Message)
            End Try

        End If

        Return 0
    End Function

    '***************************************
    '* An attempt to initiate DH485 comms
    '***************************************
    Private Sub SendStart()
        OpenComms()
        Dim a() As Byte = {m_TargetNode + &H80, &H2, m_MyNode + &H80} '* This is an initial packet sent from RSLinx
        IncrementTNS()

        SendData(a)

        'm_TargetNode += 1

    End Sub

    '*******************************************************************
    '* Send Data - this is the key entry used by the application layer
    '* A command stream in the form a a list of bytes are passed
    '* to this method. Protocol commands are then attached and
    '* then sent to the serial port
    '*******************************************************************
    Private Acknowledged As Boolean
    Private NotAcknowledged As Boolean
    Private AckWaitTicks As Integer
    Private ACKed(255) As Boolean
    Private ReadOnly MaxSendRetries As Integer = 2

    Private Function SendData(ByVal data() As Byte) As Integer
        '* A USB converer may need this
        'System.Threading.Thread.Sleep(50)

        '* Make sure there is data to send
        If data.Length < 1 Then Return -7


        If Not SerialPort.IsOpen Then
            Dim OpenResult As Integer = OpenComms()
            If OpenResult <> 0 Then Return OpenResult
        End If


        '***************************************
        '* Calculate CheckSum of raw data
        '***************************************
        Dim CheckSumCalc As UInt16
        If m_CheckSum = CheckSumOptions.Crc Then
            CheckSumCalc = CalculateCRC16(data)
        Else
            CheckSumCalc = CalculateBCC(data)
        End If

        '***********************************************************
        '* Replace any 16's (DLE's) in the data string with a 16,16
        '***********************************************************
        Dim FirstDLE As Integer = Array.IndexOf(data, CByte(16))
        If FirstDLE >= 0 Then
            Dim i As Integer = FirstDLE
            While i < data.Length   ' removed the -1 because the last byte could have been 16 30-AUG-07
                If data(i) = CByte(16) Then
                    ReDim Preserve data(data.Length)
                    For j As Integer = data.Length - 1 To i + 1 Step -1
                        data(j) = data(j - 1)
                    Next
                    'data.Insert(i, 16)
                    i += 1
                End If
                i += 1
            End While
        End If


        Dim ByteCount As Integer = data.Length + 5

        '*********************************
        '* Attach STX, ETX and Checksum
        '*********************************
        Dim BytesToSend(ByteCount) As Byte
        BytesToSend(0) = 16     '* DLE
        BytesToSend(1) = 2      '* STX

        data.CopyTo(BytesToSend, 2)

        BytesToSend(ByteCount - 3) = 16 '* DLE
        BytesToSend(ByteCount - 2) = 3  '* ETX


        BytesToSend(ByteCount - 1) = (CheckSumCalc And 255)
        BytesToSend(ByteCount) = CheckSumCalc >> 8



        '*********************************************
        '* Send the data and retry 3 times if failed
        '*********************************************
        '* Prepare for response and retries
        Dim Retries As Integer = 0

        NotAcknowledged = True
        Acknowledged = False
        'While NotAcknowledged And Retries < 2
        While Not Acknowledged And Retries < MaxSendRetries '* Changed 18-FEB-08, ARJ
            If m_Protocol <> "DF1" Then
                SerialPort.RtsEnable = True
                SerialPort.DtrEnable = False
            End If


            '* Reset the response for retries
            Acknowledged = False
            NotAcknowledged = False

            '*******************************************************
            '* The stream of data is complete, send it now
            '* For those who want examples of data streams, put a
            '*  break point here nd watch the BytesToSend variable
            '*******************************************************
            SerialPort.Write(BytesToSend, 0, BytesToSend.Length)


            'System.Threading.Thread.Sleep(10)
            '* This is a test to try to get DH485 to work with PIC module
            If m_Protocol <> "DF1" Then
                SerialPort.RtsEnable = False
                SerialPort.DtrEnable = True
            End If


            '* Wait for response of a 1 second (50*20) timeout
            '* We only wait need to wait for an ACK
            '*  The PrefixAndSend Method will continue to wait for the data
            If m_Protocol = "DF1" Then
                AckWaitTicks = 0
                While (Not Acknowledged And Not NotAcknowledged) And AckWaitTicks < MaxTicks
                    System.Threading.Thread.Sleep(20)
                    AckWaitTicks += 1
                    'If Response = ResponseTypes.Enquire Then Response = ResponseTypes.NoResponse
                End While

                '* TODO : check to see if NAK will cause a retry
                If NotAcknowledged Or AckWaitTicks >= MaxTicks Then
                    Dim DebugCheck As Integer = 0
                End If
            Else
                'Response = ResponseTypes.Acknowledged
            End If

            Retries += 1
        End While


        '**************************************
        '* Return a code indicating the status
        '**************************************
        If Acknowledged Then
            Return 0
        ElseIf NotAcknowledged Then
            Return -2  '* Not Acknowledged
        Else
            Return -3  '* No Response
        End If
        'Select Case Response
        '    Case ResponseTypes.Acknowledged : Return 0
        '    Case ResponseTypes.DataReturned : Return 0
        '    Case ResponseTypes.NotAcknowledged : Return -2
        '    Case ResponseTypes.NoResponse : Return -3
        '    Case Else : Return -4
        'End Select
    End Function

    ''' <summary>
    ''' Closes the comm port
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub CloseComms()
        If SerialPort.IsOpen Then
            'Try
            SerialPort.DiscardInBuffer()
            SerialPort.Close()
            'Catch ex As Exception
            'End Try
        End If
    End Sub

    '***********************************************
    '* Clear the buffer on a framing error
    '* This is an indication of incorrect baud rate
    '* If PLC is in DH485 mode, serial port throws
    '* an exception without this
    '***********************************************

    Private Sub DF1Comm_DataReceived(sender As Object, e As EventArgs) Handles Me.DataReceived

    End Sub

    Private Sub DF1Comm_AutoDetectTry(sender As Object, e As EventArgs) Handles Me.AutoDetectTry

    End Sub

#End Region

End Class


'*************************************************
'* Create an exception class for the DF1 class
'*************************************************
<SerializableAttribute()> _
Public Class DF1Exception
    Inherits Exception

    Public Sub New()
        '* Use the resource manager to satisfy code analysis CA1303
        Me.New(New System.Resources.ResourceManager("en-US", System.Reflection.Assembly.GetExecutingAssembly()).GetString("DF1 Exception"))
    End Sub

    Public Sub New(ByVal message As String)
        Me.New(message, Nothing)
    End Sub

    Public Sub New(ByVal innerException As Exception)
        Me.New(New System.Resources.ResourceManager("en-US", System.Reflection.Assembly.GetExecutingAssembly()).GetString("DF1 Exception"), innerException)
    End Sub

    Public Sub New(ByVal message As String, ByVal innerException As Exception)
        MyBase.New(message, innerException)
    End Sub

    Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
        MyBase.New(info, context)
    End Sub
End Class
