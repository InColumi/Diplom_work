using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Diplom_designed
{
    public partial class StartMenu
    {
        string MethodLineShifr(string lenguage, string encodeDecode, ref string text)
        {
            int sizeText = text.Length;
            int[] indexText = new int[sizeText];
            switch(lenguage)
            {
                case "RUS":
                {
                    GetIndexText(ref indexText, ref _alfaBetRussian, ref text, ref sizeText);
                    switch(encodeDecode)
                    {
                        case "ENCODE":
                        {
                            return LineShifDeshifr(ref indexText, ref _alfaBetRussian, ref sizeText, "ENCODE");
                        }

                        case "DECODE":
                        {
                            labelShifr.Text = $"Символы: {indexText.Length}";
                            return LineShifDeshifr(ref indexText, ref _alfaBetRussian, ref sizeText, "DECODE");
                        }

                    }
                    return "EROR TEXT!";
                }
                case "ENG":
                {
                    GetIndexText(ref indexText, ref _alfaBetEnglish, ref text, ref sizeText);
                    switch(encodeDecode)
                    {
                        case "ENCODE":
                        {
                            return LineShifDeshifr(ref indexText, ref _alfaBetEnglish, ref sizeText, "ENCODE");
                        }

                        case "DECODE":
                        {
                            labelShifr.Text = $"Символы: {indexText.Length}";
                            return LineShifDeshifr(ref indexText, ref _alfaBetEnglish, ref sizeText, "DECODE");
                        }

                    }
                    return "EROR TEXT!";
                }
                default:
                {
                    return "EROR TEXT!";
                }
            }
        }

        string LineShifDeshifr(ref int[] indexText, ref string lenguage, ref int textSize, string encodeDecode)
        {
            Matrix<double> matrixForMull = new DenseMatrix(_globalSizeMatrix, 1);
            int countBeegram = textSize / _globalSizeMatrix;
            int sizeLenguage = lenguage.Length;
            int tempCountBeegram;
            StringBuilder stringBuilder = new StringBuilder(textSize);
            switch(encodeDecode)
            {
                case "ENCODE":
                    StartProgressBar(progressBarText, countBeegram);
                    for(int i = 0; i < countBeegram; ++i)
                    {
                        tempCountBeegram = (_globalSizeMatrix * i);
                        for(int j = tempCountBeegram; j < _globalSizeMatrix + tempCountBeegram; ++j)
                            matrixForMull[j % _globalSizeMatrix, 0] = indexText[j];

                        matrixForMull = _globalMatrix * matrixForMull;
                        GetMod(ref matrixForMull, sizeLenguage);

                        for(int k = 0; k < _globalSizeMatrix; ++k)
                            stringBuilder.Append(lenguage[Convert.ToInt16(matrixForMull[k, 0])]);
                        ++progressBarText.Value;
                    }
                    break;
                case "DECODE":
                    Matrix<double> matrixRevers = new DenseMatrix(_globalSizeMatrix, _globalSizeMatrix);//Обратная матрица
                    int detMatrixRevers;
                    detMatrixRevers = ReversNumber(_matrDet, sizeLenguage);
                    if(detMatrixRevers < 0)
                        detMatrixRevers += sizeLenguage;

                    MatrixForDeShifr(ref matrixRevers);//Разложение матрицы по минорам
                    matrixRevers = matrixRevers.Transpose();                //Транспонирование матрицы 
                    matrixRevers *= detMatrixRevers;                        //умножение матрицы на обратное чилсло к определителю матрицы А

                    GetMod(ref matrixRevers, sizeLenguage);

                    Matrix<double> matrIndexText = new DenseMatrix(_globalSizeMatrix, 1);
                    StartProgressBar(progressBarText, countBeegram);

                    for(int i = 0; i < countBeegram; ++i)
                    {
                        tempCountBeegram = (_globalSizeMatrix * i);
                        for(int j = tempCountBeegram; j < _globalSizeMatrix + tempCountBeegram; ++j)
                            matrIndexText[j % _globalSizeMatrix, 0] = indexText[j];

                        matrixForMull = (matrixRevers * matrIndexText) % sizeLenguage;//Формула Дешифрования

                        for(int k = 0; k < _globalSizeMatrix; ++k)
                            stringBuilder.Append(lenguage[Convert.ToInt16(matrixForMull[k, 0])]);
                        
                        ++progressBarText.Value;
                    }
                    break;
                default:
                {
                    return "EROR TEXT!";
                }
            }
            RestartProgressBar(progressBarText);
            labelShifr.Text = $"Символы: {richTextBoxOutput.Text.Length}";
            return stringBuilder.ToString();
        }

        void MatrixForDeShifr(ref Matrix<double> matr)
        {
            for(int i = 0; i < _globalSizeMatrix; ++i)
                for(int j = 0; j < _globalSizeMatrix; ++j)
                    matr[i, j] = (((i + j + 2) % 2 == 0) ? 1 : -1) * DeleteRowsColsAndFindDeterminant(i, j);
        }

        int DeleteRowsColsAndFindDeterminant(int it, int jt)
        {
            Matrix<double> matr = new DenseMatrix(_globalSizeMatrix - 1, _globalSizeMatrix - 1);

            int deli = 0;
            int delj;

            for(int i = 0; i < _globalSizeMatrix - 1; i++)
            {
                if(i == it)
                {
                    deli = 1;
                }
                delj = 0;
                for(int j = 0; j < _globalSizeMatrix - 1; j++)
                {
                    if(j == jt)
                    {
                        delj = 1;
                    }
                    matr[i, j] = _globalMatrix[i + deli, j + delj];
                }
            }
            return Convert.ToInt32(matr.Determinant());
        }
        void GetMod(ref Matrix<double> A, int alfLength)
        {
            int col = A.ColumnCount;
            int row = A.RowCount;
            for(int i = 0; i < row; ++i)
            {
                for(int j = 0; j < col; ++j)
                {
                    A[i, j] = Mod(Convert.ToInt16(A[i, j]), alfLength);
                }
            }
        }

        void GetIndexText(ref int[] ans, ref string lenguage, ref string text, ref int textSize)
        {
            int sizeLenguage = lenguage.Length;
            for(int i = 0; i < textSize; ++i)
            {
                for(int j = 0; j < sizeLenguage; ++j)
                {
                    if(text[i] == lenguage[j])
                    {
                        ans[i] = j;
                        break;
                    }
                }
            }
        }

        void DeleteUsefulSymbol(ref string text, string lenguage, ref int sizeText)
        {
            StringBuilder stringBuilder = new StringBuilder(sizeText);
            switch(lenguage)
            {
                case "RUS":
                {
                    FindUsefulSumbol(ref stringBuilder, ref text, sizeText, _sizeRusAlf, ref _alfaBetRussian);
                    break;
                }
                case "ENG":
                {
                    FindUsefulSumbol(ref stringBuilder, ref text, sizeText, _sizeEngAlf, ref _alfaBetEnglish);
                    break;
                }
                default:
                    break;
            }
            text = stringBuilder.ToString();
        }

        StringBuilder FindUsefulSumbol(ref StringBuilder stringBuilder, ref string text, int sizeText, int sizeLenguage, ref string lenguage)
        {
            for(int i = 0; i < sizeText; ++i)
            {
                for(int j = 0; j < sizeLenguage; ++j)
                {
                    if(text[i] == lenguage[j])
                    {
                        stringBuilder.Append(text[i]);
                        // ans += text[i];
                        break;
                    }
                }
            }
            return stringBuilder;
        }
    }
}
