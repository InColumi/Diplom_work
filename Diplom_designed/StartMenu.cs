using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Diplom_designed
{
    public partial class StartMenu:Form
    {
        private bool _getMatrix = false;                                                 // Получение размерности матрицы
        private bool _checkSymbolInMatrix = false;                                       // Проверка символов матрицы
        private bool _goodMatrix = false;                                                // Матрица Не прошла проверку
        private bool _switchingLanguage = true;                                          // Переключение языка
        private bool _editCell = false;                                                  // матрицу изменили
        private bool _genarateAlfabet = false;
                                    
        private static string _alfaBetEnglish = "ABCDEFGHIJKLMNOPQRSTUVWXYZ .,";         // Английский алфавит + 3 символа 
        private static string _alfaBetRussian = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ., ;"; // Русский алфавит + 4 символа
        private static readonly int _sizeEngAlf = _alfaBetEnglish.Length;
        private static readonly int _sizeRusAlf = _alfaBetRussian.Length;
        private static int _globalSizeMatrix;                                            // Размерность матрицы
        
        private int _matrDet;
        private static int _global_b;
        private static int _global_c;
        private static int _global_modP;

        private Matrix<double> _globalMatrix;

        private string _path; // Путь файла с алфавитом.

        private Dictionary<int, Point> _alfabetElliptic = new Dictionary<int, Point>(); // матрица

        private readonly Color colorTexBox = Color.FromArgb(40, 42, 55);

         public StartMenu()
        {
            InitializeComponent();
            EventInputText();
        }

        private void EventInputText()
        {
            textBoxB.KeyPress += (s, e) =>
            {
                CheckColor(textBoxB);
                CheckPressSymbol(textBoxB, e);
            };
            textBoxC.KeyPress += (s, e) =>
            {
                CheckColor(textBoxC);
                CheckPressSymbol(textBoxC, e);
            };
            textBoxP.KeyPress += (s, e) =>
            {
                CheckColor(textBoxP);
                CheckPressSymbol(textBoxP, e, false);
            };
            textBoxN.KeyPress += (s, e) =>
            {
                CheckColor(textBoxN);
                CheckPressSymbol(textBoxN, e, false);
            };
            textBoxK.KeyPress += (s, e) =>
            {
                CheckColor(textBoxK);
                CheckPressSymbol(textBoxN, e, false);
            };

            buttonInputMatrix.Click += (s, e) => { InputMatrix(); };

            buttonClearText.Click += (s, e) =>
            {
                richTextBoxInput.Clear();
                labelText.Text = $"Символы: {richTextBoxInput.Text.Length}";
            };

            buttonCheckMatrix.Click += (s, e) =>
            {
                if(_getMatrix)
                    CheckMatrix();
                else
                    MessageBox.Show("Задайте матрицу!");
            };

            buttonClearShifr.Click += (s, e) =>
            {
                richTextBoxOutput.Clear();
                labelShifr.Text = $"Символы: {richTextBoxOutput.Text.Length}";
            };

            richTextBoxInput.TextChanged += (s, e) => { labelText.Text = $"Символы: {richTextBoxInput.Text.Length}"; };
        }

        private void CheckColor(TextBox textBox)
        {
            if(textBox.BackColor != colorTexBox)
                textBox.BackColor = colorTexBox;
        }

        private void CheckPressSymbol(TextBox textBox, KeyPressEventArgs e, bool minus = true)
        {
            char number = e.KeyChar;
            if(textBox.Text == string.Empty && minus == true)
            {
                if(!Char.IsDigit(number) && number != 8 && number != 45) // минус
                {
                    e.Handled = true;
                }
            }
            else
            {
                if(!Char.IsDigit(number) && number != 8) // цифры и клавиша BackSpace
                {
                    e.Handled = true;
                }
            }

        }

        private void TextBoxSizeMatrix_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
                InputMatrix();
            else
                CheckPressSymbol(textBoxSizeMatrix, e, false);
        }

        private void InputMatrix()
        {
            if(textBoxSizeMatrix.Text == "0" || textBoxSizeMatrix.Text == "00" || textBoxSizeMatrix.Text == string.Empty || textBoxSizeMatrix.Text == "1")
            {
                MessageBox.Show("Введите корректный размер матрицы!");
            }
            else
            {
                try
                {
                    _globalSizeMatrix = Convert.ToInt32(textBoxSizeMatrix.Text);
                    _globalMatrix = new DenseMatrix(_globalSizeMatrix);
                    _getMatrix = true;
                    _goodMatrix = false;
                    _editCell = false;
                    _checkSymbolInMatrix = false;
                    dataGridViewMatrix.RowCount = _globalSizeMatrix;
                    dataGridViewMatrix.ColumnCount = _globalSizeMatrix;
                }
                catch
                {
                    MessageBox.Show("Введите корректный размер матрицы!");
                }

            }
        }

        private void CheckMatrix()
        {
            _globalMatrix = GetMatr();

            if(_checkSymbolInMatrix)
            {
                _matrDet = Convert.ToInt32(_globalMatrix.Determinant());
                if(_matrDet == 0)
                    MessageBox.Show("Определитель матрицы не подходит для выбранного языка.\nИзмените язык или матрицу!");
                else
                {
                    if(radioButtonEng.Checked)
                    {
                        CheckMatrixRadioButton(_sizeEngAlf, radioButtonEng);
                    }
                    else
                    {
                        CheckMatrixRadioButton(_sizeRusAlf, radioButtonRus);
                    }
                    _goodMatrix = true;
                }
            }
        }

        private void CheckMatrixRadioButton(int sizeAlfabet, RadioButton radioButton)
        {
            _matrDet = Mod(_matrDet, sizeAlfabet);
            if(Nod(_matrDet, sizeAlfabet) == 1)
            {
                MessageBox.Show($"Матрица подходит.\nВы выбрали: {radioButton.Text}");
                _switchingLanguage = false;
            }
            else
                MessageBox.Show("Определитель матрицы не подходит для выбранного языка.\nИзмените язык или матрицу!");
        }

        private Matrix<double> GetMatr()
        {
            for(int i = 0; i < _globalSizeMatrix; i++)
            {
                for(int j = 0; j < _globalSizeMatrix; j++)
                {
                    try
                    {
                        int val_int = int.Parse(dataGridViewMatrix.Rows[i].Cells[j].Value.ToString());
                        _globalMatrix[i, j] = val_int;
                        dataGridViewMatrix.Rows[i].Cells[j].Value = val_int;
                    }
                    catch
                    {
                        dataGridViewMatrix.Rows[i].Cells[j].Value = "0";
                        MessageBox.Show("Введите корректные значения в матрицу!");
                        return new DenseMatrix(_globalSizeMatrix);
                    }
                }
            }
            _checkSymbolInMatrix = true;
            _editCell = true;
            return _globalMatrix;
        }

        private void EditText(ref string text, string lenguage, string encodeDecode, ref EllipticParameters ellipticParameters, ref List<Point> pointsEllipticCurve)
        {
            int sizeText = richTextBoxInput.Text.Length;

            DeleteUsefulSymbol(ref text, lenguage, ref sizeText);
            sizeText = text.Length;
            int countAddSymbols = sizeText % _globalSizeMatrix;

            AddNewSymbol(countAddSymbols, lenguage, ref text);
            sizeText = text.Length;
            string encodeLineShifr = string.Empty;
            //string encodeLineShifr = text;
            switch(encodeDecode)
            {
                case "ENCODE":
                    encodeLineShifr = MethodLineShifr(lenguage, encodeDecode, ref text);
                    EncodeDecodeElliptic(lenguage, ref encodeLineShifr, ellipticParameters, encodeDecode, ref pointsEllipticCurve);
                    break;
                case "DECODE":
                    EncodeDecodeElliptic(lenguage, ref encodeLineShifr, ellipticParameters, encodeDecode, ref pointsEllipticCurve);
                    encodeLineShifr = MethodLineShifr(lenguage, encodeDecode, ref encodeLineShifr);
                    break;
            }
            richTextBoxOutput.AppendText(encodeLineShifr);
            labelShifr.Text = richTextBoxOutput.Text.Length.ToString("Символы: #");
        }

        private bool CheckSimpleNumbersInElliptic(ref List<Point> pointsEllipticCurve, ref Point point, ref int simpleNumber)
        {
            progressBarKeys.Maximum = _global_modP;
            progressBarKeys.Visible = true;
            for(int i = _global_modP - 1; i >= 0; --i)
            {
                for(int j = 0; j < pointsEllipticCurve.Count(); j++)
                {
                    Point a = pointsEllipticCurve[j] * i;
                    if(a.CheckInfinityZero() && CheckSimpleNumberMethodFerma(i))
                    {
                        simpleNumber = i;
                        progressBarKeys.Value = 0;
                        progressBarKeys.Visible = false;
                        return true;
                    }
                }
                ++progressBarKeys.Value;
            }
            progressBarKeys.Value = 0;
            progressBarKeys.Visible = false;
            for(int i = 2; i < _global_modP; ++i)
            {
                if((pointsEllipticCurve[i] * i).CheckInfinityZero() != true)
                {
                    point = pointsEllipticCurve[i];
                    break;
                }
            }
            return false;
        }

        private void CheckEmptyOpenKeys(ref EllipticParameters ellipticParameters, ref List<Point> pointsEllipticCurve, Point generatedG = null, int simpleNumber = 0)
        {
            Random random = new Random();
            if(textBoxN.Text != string.Empty)
            {
                ellipticParameters.Nb = Convert.ToInt32(textBoxN.Text);
                if(ellipticParameters.Nb >= _global_modP)
                {
                    MessageBox.Show($"Ключ \"n\" должен быть меньше {_global_modP}!");
                    textBoxN.Clear();
                }
            }
            else if(textBoxN.Text == string.Empty && generatedG == null)
            {
                ellipticParameters.Nb = (random.Next(2, _global_modP - 1));
                textBoxN.Text = ellipticParameters.Nb.ToString();
            }
            else if(textBoxN.Text == string.Empty && generatedG != null)
            {
                ellipticParameters.Nb = simpleNumber;
                textBoxN.Text = simpleNumber.ToString();
            }

            if(textBoxK.Text != string.Empty)
            {
                ellipticParameters.K = Convert.ToInt32(textBoxK.Text);
                if(ellipticParameters.K >= _global_modP)
                {
                    MessageBox.Show($"Ключ \"k\" должен быть меньше {_global_modP}!");
                    textBoxK.Clear();
                }
            }
            else
            {
                ellipticParameters.K = _global_modP - 1;
                textBoxK.Text = (random.Next(2, _global_modP - 1)).ToString();
            }

            if(textBoxG.Text != string.Empty)
            {
                Point g = new Point();
                if(GetPointG(ref g))
                {
                    ellipticParameters.G = g;
                    if(!CheckPointG(ellipticParameters.G, ref pointsEllipticCurve))
                    {
                        MessageBox.Show("Точка G не принадлежит множеству!");
                        textBoxG.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("Введите корректные значения точки G!");
                }
            }
            else
            {
                ellipticParameters.G = pointsEllipticCurve[random.Next(0, pointsEllipticCurve.Count())];
                textBoxG.Text = ellipticParameters.G.ToString();
            }
            ellipticParameters.SetParameters();
            textBoxPb.Text = ellipticParameters.Pb.ToString();
        }

        private void AddNewSymbol(int countAddSymbols, string lenguage, ref string text)
        {
            Random rand = new Random();
            StringBuilder stringBuilder = new StringBuilder(countAddSymbols);
            switch(lenguage)
            {
                case "RUS":
                {
                    if(countAddSymbols < _globalSizeMatrix)
                    {
                        while(countAddSymbols != 0)
                        {
                            stringBuilder.Append(_alfaBetRussian[rand.Next(0, _sizeRusAlf)]);
                            --countAddSymbols;
                        }
                    }
                    break;
                }
                case "ENG":
                {
                    if(countAddSymbols < _globalSizeMatrix)
                    {
                        while(countAddSymbols != 0)
                        {
                            stringBuilder.Append(_alfaBetEnglish[rand.Next(0, _sizeEngAlf)]);
                            --countAddSymbols;
                        }
                    }
                    break;
                }
            }
            text += stringBuilder.ToString();
        }

        private bool CheckEllipticParameters()
        {
            List<TextBox> textBoxes = new List<TextBox>() { textBoxB, textBoxC, textBoxP };
            foreach(TextBox textBox in textBoxes)
            {
                if(textBox.Text == string.Empty)
                {
                    textBox.BackColor = Color.Red;
                    return false;
                }
            }
            try
            {
                _global_b = Convert.ToInt32(textBoxB.Text);
                _global_c = Convert.ToInt32(textBoxC.Text);
                _global_modP = Convert.ToInt32(textBoxP.Text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetPointG(ref Point point)
        {
            string pointG = textBoxG.Text;
            string[] splitPointG = pointG.Split(',');
            try
            {
                point = new Point(
                    Convert.ToInt32(splitPointG[0]),
                    Convert.ToInt32(splitPointG[1])
                    );
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void ReplaceCountSymbols()
        {
            richTextBoxInput.Text = richTextBoxOutput.Text;
            richTextBoxOutput.Clear();
            labelShifr.Text = $"Символы: {0}";
            labelText.Text = $"Символы: {richTextBoxInput.Text.Length}";
        }

        private void En_DeCodeText(string encodeDecode)
        {
            if(_goodMatrix && _checkSymbolInMatrix && _editCell && _switchingLanguage == false)
            {
                if(richTextBoxInput.Text != string.Empty)
                {
                    if(richTextBoxOutput.Text != string.Empty)
                    {
                        DialogResult result = MessageBox.Show(
                            "Поле Шифр не пустое!\nУдалить и продолжить ?",
                            "Очистка поля Шифр.",
                            MessageBoxButtons.YesNo
                        );
                        if(result == DialogResult.Yes)
                        {
                            richTextBoxOutput.Clear();
                            labelShifr.Text = $"Символы: {0}";
                        }
                        else
                            return;
                    }

                    if(!CheckEllipticParameters())
                    {
                        MessageBox.Show("Введите корректные параметры\nдля эллиптического шифра!");
                        return;
                    }
                    if(_alfabetElliptic.Count == 0)
                    {

                    }

                    EllipticParameters ellipticParameters = new EllipticParameters();
                    List<Point> pointsEllipticCurve = new List<Point>();
                    FindPointForCurve(ref pointsEllipticCurve);
                    Point g = new Point();
                    int n = 0;
                    if(CheckSimpleNumbersInElliptic(ref pointsEllipticCurve, ref g, ref n))
                    {
                        CheckEmptyOpenKeys(ref ellipticParameters, ref pointsEllipticCurve, g, n);
                    }
                    else
                    {
                        CheckEmptyOpenKeys(ref ellipticParameters, ref pointsEllipticCurve);
                    }

                    int sizeText = richTextBoxInput.Text.Length;
                    if(sizeText >= 2147483647)
                        MessageBox.Show("Введенный текст слишком большой.\nТекст будет ограничен до 2147483647 символов!");

                    string text = richTextBoxInput.Text.ToUpper();

                    if(radioButtonRus.Checked)
                    {
                        EditText(ref text, "RUS", encodeDecode, ref ellipticParameters, ref pointsEllipticCurve);

                    }
                    else
                    {
                        EditText(ref text, "ENG", encodeDecode, ref ellipticParameters, ref pointsEllipticCurve);
                    }
                }
                else
                {
                    MessageBox.Show("Введите текст.");
                }
                   
            }
            else
            {
                MessageBox.Show("Матрица не прошла проверку или была изменена.\nПродолжить не возможно!\nПроверте матрицу!");
            }
                
        }

        private void DataGridViewMatrix_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            _checkSymbolInMatrix = false;
            _goodMatrix = false;
            _editCell = false;
        }

        private void ButtonShifrovanie_Click(object sender, EventArgs e)
        {
            //Stopwatch sWatch = new Stopwatch();
            //sWatch.Start();
            //любой набор операций (работа с базой данных)
            En_DeCodeText("ENCODE");
            //sWatch.Stop();
            //MessageBox.Show($"Время работы: {sWatch.ElapsedMilliseconds / 1000}");
        }

        private void ButtonDeShifrovanie_Click(object sender, EventArgs e)
        {
            // Stopwatch sWatch = new Stopwatch();
            // sWatch.Start();
            //любой набор операций (работа с базой данных)
            En_DeCodeText("DECODE");
            //sWatch.Stop();
            //MessageBox.Show($"Время работы: {sWatch.ElapsedMilliseconds/1000}");
        }

        private void ButtonPushUp_Click(object sender, EventArgs e)
        {

            if(richTextBoxInput.Text != string.Empty && richTextBoxOutput.Text != string.Empty)
            {
                DialogResult result = MessageBox.Show(
                "Поле Текст не пустое!\nУдалить и переместить ?",
                "Перемещение текста.",
                MessageBoxButtons.YesNo
                );
                if(result == DialogResult.Yes)
                {
                    ReplaceCountSymbols();
                }
            }
            else
            {
                ReplaceCountSymbols();
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(_switchingLanguage == false)
            {
                DialogResult result = MessageBox.Show(
                    "При переключении языка потребуется проверка матрицы!\nПродолжить ?",
                    "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question
                    );
                if(result == DialogResult.Yes)
                {
                    _switchingLanguage = true;
                }
                else
                {
                    return;
                }
            }
        }

        private void TextBoxG_KeyPress(object sender, KeyPressEventArgs e)
        {
            char number = e.KeyChar;
            if(textBoxG.Text != string.Empty)
            {
                // coma and numbers and backspace
                if(number != 44 && !Char.IsDigit(number) && number != 8)
                    e.Handled = true;
            }
            else
            {
                // numbers and backspace
                if(!Char.IsDigit(number) && number != 8)
                    e.Handled = true;
            }
            textBoxG.BackColor = colorTexBox;


        }

        private async void SaveAlfaBettoolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_alfabetElliptic.Count != 0)
            {
                using(SaveFileDialog fileDialog = new SaveFileDialog())
                {
                    fileDialog.Title = "Сохранение алфавита"; // заголовок диалогового окна
                    fileDialog.DefaultExt = "txt"; // расширение по умолчанию
                    fileDialog.AddExtension = true; // при значении true добавляет к имени файла расширение при его отсуствии. Расширение берется из свойства DefaultExt или Filter
                    fileDialog.Filter = "Текстовые файлы(*.txt)|*.txt"; // задает фильтр файлов, благодаря чему в диалоговом окне можно отфильтровать файлы по расширению. Фильтр задается в следующем формате Название_файлов|*.расширение. Например, Текстовые файлы(*.txt)|*.txt. Можно задать сразу несколько фильтров, для этого они разделяются вертикальной линией |. Например, Bitmap files (*.bmp)|*.bmp|Image files (*.jpg)|*.jpg
                    fileDialog.CreatePrompt = false;// при значении true в случае, если указан не существующий файл, то будет отображаться сообщение о его создании
                    fileDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString(); //

                    if(fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        _path = fileDialog.FileName;
                        //Stream fileStream = fileDialog.OpenFile();
                        using(StreamWriter writer = new StreamWriter(_path))
                        {
                            int size = _alfabetElliptic.Count;
                            for(int i = 0; i < size; ++i)
                            {
                                await writer.WriteAsync(_alfabetElliptic[i].ToString() + '*');
                            }

                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Алфавит не сгенерирован.\nСохранять нечего.",
                                "Недостаточно данных",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }
        }

        private async void OpenFilewithAlfabetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Открыть файл с алфавитом"; // заголовок диалогового окна
                //openFileDialog.DefaultExt = "txt"; // расширение по умолчанию
                openFileDialog.AddExtension = true; // при значении true добавляет к имени файла расширение при его отсуствии. Расширение берется из свойства DefaultExt или Filter
                openFileDialog.Filter = "Текстовые файлы(*.txt)|*.txt"; // задает фильтр файлов, благодаря чему в диалоговом окне можно отфильтровать файлы по расширению. Фильтр задается в следующем формате Название_файлов|*.расширение. Например, Текстовые файлы(*.txt)|*.txt. Можно задать сразу несколько фильтров, для этого они разделяются вертикальной линией |. Например, Bitmap files (*.bmp)|*.bmp|Image files (*.jpg)|*.jpg
                if(_path == "")
                {
                    openFileDialog.InitialDirectory = _path;
                }
                else
                {
                    openFileDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString(); //
                }
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _path = openFileDialog.FileName;
                    string abFromFile;
                    if(IsEmptyAlfabetElliptic())
                    {
                        using(StreamReader reader = new StreamReader(_path))
                        {
                            abFromFile = await reader.ReadToEndAsync();
                        }
                        string[] strSplit = abFromFile.Split('*');
                        int size = strSplit.Length;
                        for(int i = 0; i < size - 1; ++i)
                        {
                            string[] strPoint = strSplit[i].Split(',');
                            Point point = new Point(Convert.ToInt32(strPoint[0]), Convert.ToInt32(strPoint[1]));
                            _alfabetElliptic.Add(i, point);
                        }
                        if(_alfabetElliptic.Count == _alfaBetRussian.Length)
                        {
                            SetRadioBtn(true);
                        }
                        else if(_alfabetElliptic.Count == _alfaBetEnglish.Length)
                        {
                            SetRadioBtn(false);
                        }
                        else
                        {
                            MessageBox.Show("Алфафит поврежден или отличается от русского или английского.\nПродолжить невозможно.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            _alfabetElliptic.Clear();
                        }
                    }
                }
            }
        }

        private bool IsEmptyAlfabetElliptic()
        {
            if(_alfabetElliptic.Count != 0)
            {
                DialogResult dialogResult = MessageBox.Show("В программе уже есть алфвит!\nПерезаписать ?...", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.Yes)
                {
                    _alfabetElliptic.Clear();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void SetRadioBtn(bool flag)
        {
            if(flag)
            {
                radioButtonRus.Checked = true;
                radioButtonEng.Checked = false;
            }
            else
            {
                radioButtonRus.Checked = false;
                radioButtonEng.Checked = true;
            }

        }

        private async void upLoadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Загрузка текста";
                openFileDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString();
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Тектовые файлы(*.txt)|*.txt";
                if(openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if(richTextBoxInput.Text != "")
                    {
                        if(MessageBoxShow("Поле текст не пустое!\nОчистить и продолжить?", "Поле Текст", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            richTextBoxInput.Clear();
                        else
                            return;
                    }
                    using(StreamReader reader = new StreamReader(openFileDialog.FileName))
                    {
                        richTextBoxInput.Text = await reader.ReadToEndAsync();
                    }
                }
            }
        }

        private bool MessageBoxShow(string t, string c, MessageBoxButtons button, MessageBoxIcon icon)
        {
            DialogResult dialogResult = MessageBox.Show(t, c, button, icon);
            if(dialogResult == DialogResult.Yes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void saveFileToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(richTextBoxOutput.Text != "")
            {
                using(SaveFileDialog fileDialog = new SaveFileDialog())
                {
                    fileDialog.Title = "Сохранение шифра"; // заголовок диалогового окна
                    fileDialog.DefaultExt = "txt"; // расширение по умолчанию
                    fileDialog.AddExtension = true; // при значении true добавляет к имени файла расширение при его отсуствии. Расширение берется из свойства DefaultExt или Filter
                    fileDialog.Filter = "Текстовые файлы(*.txt)|*.txt"; // задает фильтр файлов, благодаря чему в диалоговом окне можно отфильтровать файлы по расширению. Фильтр задается в следующем формате Название_файлов|*.расширение. Например, Текстовые файлы(*.txt)|*.txt. Можно задать сразу несколько фильтров, для этого они разделяются вертикальной линией |. Например, Bitmap files (*.bmp)|*.bmp|Image files (*.jpg)|*.jpg
                    fileDialog.CreatePrompt = false;// при значении true в случае, если указан не существующий файл, то будет отображаться сообщение о его создании
                    fileDialog.InitialDirectory = Environment.SpecialFolder.Desktop.ToString(); //
                    if(fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        using(StreamWriter writer = new StreamWriter(fileDialog.FileName))
                        {
                            await writer.WriteAsync(richTextBoxOutput.Text);
                        }
                    }
                }
            }
        }
    }
}
