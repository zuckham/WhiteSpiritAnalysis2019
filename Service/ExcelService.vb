Imports System.IO
Imports ExcelDataReader
Imports System.Data
Imports Models
Public Class ExcelService

    Public Shared Function ReadExcel(ExcelFIle As String) As DataSet

        Dim fs As FileStream = File.Open(ExcelFIle, FileMode.Open, FileAccess.Read)

            Dim reader = ExcelDataReader.ExcelReaderFactory.CreateReader(fs)
            Dim result = reader.AsDataSet()
            reader.Close()
            fs.Close()
            Return result


    End Function
    Public Shared Function ReadCsv（CsvFile As String) As DataSet
        Dim fs As FileStream = File.Open(CsvFile, FileMode.Open, FileAccess.Read)
        Dim reader = ExcelDataReader.ExcelReaderFactory.CreateCsvReader(fs, New ExcelReaderConfiguration With {
                                                                                                                        .FallbackEncoding = System.Text.Encoding.Default
        })
        Dim result = reader.AsDataSet
        reader.Close()
        fs.Close()
        Return result
    End Function


    'Public Shared Function RowToEntitiy(row As DataRow, DataType As Integer, Sample As SampleInfo) As Object
    '    If row.ItemArray.Count Then
    'End Function


End Class
