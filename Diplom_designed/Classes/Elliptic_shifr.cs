using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Diplom_designed
{
    public partial class StartMenu
    {
        class EllipticParameters
        {
            public int Nb;
            public int K;
            public Point G;
            public Point Pb;
            public Point Kpbk;
            public Point C1m;
            public Point NbC1M;
            public EllipticParameters()
            {
                Nb = 0;
                K = 0;
                G = new Point();
            }

            public void SetParameters()
            {
                Pb = G * Nb;
                Kpbk = Pb * K;
                C1m = G * K;
                NbC1M = C1m * Nb;
            }
        }

        int GetElipCurveValue(int x)
        {
            return x * x * x + _global_b * x + _global_c;
        }

        bool CheckPoints(int x, int y)
        {
            if(Mod(y * y, _global_modP) == Mod(GetElipCurveValue(x), _global_modP))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool TestModZero()
        {
            int b3 = _global_b * _global_b * _global_b;
            int c2 = _global_c * _global_c;
            if(Mod(4 * b3 + 27 * c2, _global_modP) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        void FindPointForCurve(ref List<Point> AllPoints)
        {
            if(TestModZero())
            {
                MessageBox.Show("Дискриминант равен нулю!\n");
                textBoxB.Clear();
                textBoxC.Clear();
                throw new Exception("Дискриминант равен нулю!");
            }
            //сохранение точек в массив
            for(int i = 0; i < _global_modP; ++i)
            {
                for(int j = 0; j < _global_modP; ++j)
                {
                    if(CheckPoints(i, j) == true)
                    {
                        Point temp = new Point(i, j);
                        //cout << temp << endl;;
                        AllPoints.Add(temp);
                    }
                }
            }
        }

        Dictionary<int, Point> GetElipticAlfabet(int sizeAlfabet, ref List<Point> AllPoints)
        {
            Dictionary<int, Point> a = new Dictionary<int, Point>();
            Random random = new Random();
            int sizeAllPoints = (int)AllPoints.Count();
            bool goodPoint = true;
            for(int i = 0; i < sizeAlfabet; i++)
            {
                while(true)
                {
                    int iter = random.Next(0, sizeAllPoints);
                    int j = 0;

                    while(j < a.Count())
                    {
                        Point tempPoint = AllPoints[iter];
                        if(a[j].GetX() == tempPoint.GetX() && a[j].GetY() == tempPoint.GetY() || a[j].GetX() == tempPoint.GetX() && a[j].GetY() == Mod(-tempPoint.GetY(), _global_modP))
                        {
                            goodPoint = false;
                            break;
                        }
                        ++j;
                        if(j >= a.Count())
                        {
                            goodPoint = true;
                            break;
                        }
                    }
                    if(goodPoint)
                    {
                        a[i] = AllPoints[iter];
                        goodPoint = false;
                        break;
                    }
                }
            }
            return a;
        }

        void EncodeDecodeElliptic(string lenguage, ref string text, EllipticParameters ellipticParameters, string encodeDecode, ref List<Point> points)
        {
            List<Point> textInPoints = new List<Point>();
            StringBuilder enDeCodeText = new StringBuilder();
            switch(encodeDecode)
            {
                case "ENCODE":
                {
                    enDeCodeText.Append(ellipticParameters.C1m);
                    switch(lenguage)
                    {
                        case "RUS":
                        {
                            CheckGenerateAlfabet(_sizeRusAlf, ref points);
                            LettersToPoints(ref textInPoints, ref text, _sizeRusAlf, ref _alfaBetRussian, ref _alfabetElliptic);
                            break;
                        }
                        case "ENG":
                        {
                            CheckGenerateAlfabet(_sizeEngAlf, ref points);
                            LettersToPoints(ref textInPoints, ref text, _sizeEngAlf, ref _alfaBetEnglish, ref _alfabetElliptic);
                            break;
                        }
                    }
                    foreach(Point point in textInPoints)
                    {
                        enDeCodeText.Append(point + ellipticParameters.Kpbk);
                    }
                    labelShifr.Text = $"Символы: {enDeCodeText.Length}";
                    text = enDeCodeText.ToString();
                    break;
                }
                case "DECODE":
                {
                    switch(lenguage)
                    {
                        case "RUS":
                        {
                            CheckGenerateAlfabet(_sizeRusAlf, ref points);
                            DecodeEll(ref _alfaBetRussian, ref enDeCodeText, ref _alfabetElliptic, ref ellipticParameters.Nb);
                            text = enDeCodeText.ToString();
                            break;
                        }
                        case "ENG":
                        {
                            CheckGenerateAlfabet(_sizeEngAlf, ref points);
                            DecodeEll(ref _alfaBetEnglish, ref enDeCodeText, ref _alfabetElliptic, ref ellipticParameters.Nb);
                            text = enDeCodeText.ToString();
                            break;
                        }
                    }
                    break;
                }
            }
        }

        void CheckGenerateAlfabet(int sizeText, ref List<Point> points)
        {
            if(_alfabetElliptic.Count == 0)
            {
                _alfabetElliptic = GetElipticAlfabet(sizeText, ref points);
                _genarateAlfabet = true;
            }
            else
            {
                DialogResult result = MessageBox.Show(
                            "Алфавит уже сгенерирован.\nСгенерировать заново ?",
                            "Генерация Эллиптического алфавита",
                            MessageBoxButtons.YesNo
                        );
                if(result == DialogResult.Yes)
                {
                    _alfabetElliptic = GetElipticAlfabet(sizeText, ref points);
                    _genarateAlfabet = true;
                }
            }
        }

        void DecodeEll(ref string alfabet, ref StringBuilder stringBuilder, ref Dictionary<int, Point> alfabetInPoints, ref int nB)
        {
            List<Point> pointsText = new List<Point>();
            ReadAndSplitPoint(ref pointsText);
            int countPoint = pointsText.Count();
            Point cM1 = pointsText[0];
            cM1 *= nB;
            StartProgressBar(progressBarText, countPoint);
            for(int i = 1; i < countPoint; ++i)
            {
                Point point = (pointsText[i] - cM1);
                bool check = true;
                for(int j = 0; j < alfabet.Count(); ++j)
                {
                    if(alfabetInPoints[j].GetX() == point.GetX() && alfabetInPoints[j].GetY() == point.GetY())
                    {
                        stringBuilder.Append(alfabet[j]);
                        check = false;
                        break;
                    }
                }
                if(check)
                {
                    MessageBox.Show($"Точки {point} нет в словаре!");
                }
                ++progressBarText.Value;
            }
            RestartProgressBar(progressBarText);
        }

        bool CheckPointG(Point g, ref List<Point> points)
        {
            for(int i = 0; i < points.Count(); ++i)
                if(g.GetX() == points[i].GetX() && g.GetY() == points[i].GetY() && g.CheckInfinityZero() != true)
                    return true;
            return false;
        }

        void LettersToPoints(ref List<Point> textInPoints, ref string text, int sizeAlfabet, ref string alfabet, ref Dictionary<int, Point> alfabetInPoints)
        {
            int sizeText = text.Length;
            StartProgressBar(progressBarText, sizeText);
            for(int i = 0; i < sizeText; ++i)
            {
                for(int j = 0; j < sizeAlfabet; ++j)
                {
                    if(text[i] == alfabet[j])
                        textInPoints.Add(alfabetInPoints[j]);
                }
                ++progressBarText.Value;
            }
            RestartProgressBar(progressBarText);
        }

        void ReadAndSplitPoint(ref List<Point> points)
        {
            string text = richTextBoxInput.Text;

            string[] textsplit = text.Split(',');
            int countPoints = (textsplit.Length - 1);

            for(int i = 0; i < countPoints; i += 2)
            {
                Point temp = new Point(
                    Convert.ToInt32(textsplit[i]),
                    Convert.ToInt32(textsplit[i + 1])
                    );
                points.Add(temp);
            }
        }

        void StartProgressBar(ProgressBar progressBar, int max)
        {
            progressBar.Maximum = max;
            progressBar.Visible = true;
        }

        void RestartProgressBar(ProgressBar progressBar)
        {
            progressBar.Visible = false;
            progressBar.Value = 0;
            progressBar.Maximum = 0;

        }
    }
}