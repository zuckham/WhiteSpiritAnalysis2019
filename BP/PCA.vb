Imports PCA
Imports MathNet.Numerics.LinearAlgebra.Generic
Public Class PCANet
    Shared Function Compress(Source As List(Of Double()), K As Integer, ByRef variance_retained As Double) As Object
        Dim reducer As New PCA.PCADimReducer
        Dim Out As Matrix(Of Double)
        Return reducer.CompressData(Source, K, Out, variance_retained)
    End Function

End Class



'he sample codes below shows how To use the library To reduce the number Of dimension Or reconstruct the original data from the reduced data

'List<double[]> source = GetNormalizedData();
'List<double[]> Z; // PCA output 
'double variance_retained;
'K = 5; // dimension of the Z (note that Z will have K+1 dimensions where the first dimension will be ignored)
'PCA.PCADimReducer.CompressData(source, K, out Z, out variance_retained);

'// To reconstruct some compressed data point from Z 
'List<double[]> compressed_data_point = GetCompressedDataPoints(); // K+1 dimension data points 
'List<double[]> uncompressed_data_point = ReconstructData(compressed_data_point, Z);