using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Threading;



namespace IS1_20_BorinMA
{
    public partial class Authorization : MetroFramework.Forms.MetroForm
     
    {
        string connStr= "server=chuc.caseum.ru;port=33333;user=st_1_20_4;database=is_1_20_st4_KURS;password=32006333;";

        MySqlConnection conn;

        public delegate void ThreadStart();



        static string sha256(string randomString)
        {
            // Смысл данного метода заключается в том, что строка залетает в метод
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public void GetUserInfo(string login_user)
        {
            //Объявлем переменную для запроса в БД
            string selected_id_stud = login.Text;
            // устанавливаем соединение с БД
            conn.Open();
            // запрос
            string sql = $"SELECT * FROM Auth ";
            // объект для выполнения SQL-запроса
            MySqlCommand command = new MySqlCommand(sql, conn);
            // объект для чтения ответа сервера
            MySqlDataReader reader = command.ExecuteReader();
            // читаем результат
            while (reader.Read())
            {
                // элементы массива [] - это значения столбцов из запроса SELECT
                Auth.auth_id = reader[0].ToString();
                Auth.auth_fio = reader[1].ToString();
               // Auth.auth_role = Convert.ToInt32(reader[4].ToString());
            }
            reader.Close(); // закрываем reader
            // закрываем соединение с БД
            conn.Close();
        }
        static class Auth
        {
            //Статичное поле, которое хранит значение статуса авторизации
            public static bool auth = false;
            //Статичное поле, которое хранит значения ID пользователя
            public static string auth_id = null;
            //Статичное поле, которое хранит значения ФИО пользователя
            public static string auth_fio = null;
            //Статичное поле, которое хранит количество привелегий пользователя
           // public static int auth_role = 0;
        }
        public Authorization()
        {
            InitializeComponent();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            conn = new MySqlConnection(connStr);
        }
        public void mnogopotok()
        {
            string sql = " SELECT* FROM Auth WHERE login = @un and  password = @up";
            //Открытие соединения
            conn.Open();
            //Объявляем таблицу
            DataTable table = new DataTable();
            //Объявляем адаптер
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            //Объявляем команду
            MySqlCommand command = new MySqlCommand(sql, conn);
            //Определяем параметры
            command.Parameters.Add("@un", MySqlDbType.VarChar, 25);
            command.Parameters.Add("@up", MySqlDbType.VarChar, 25);
            //Присваиваем параметрам значение
            command.Parameters["@un"].Value = login.Text;
            command.Parameters["@up"].Value = sha256(Pass.Text);
            //Заносим команду в адаптер
            adapter.SelectCommand = command;
            //Заполняем таблицу
            adapter.Fill(table);
            //Закрываем соединение
            conn.Close();

            if (table.Rows.Count > 0)
            {
                //Присваеваем глобальный признак авторизации
                Auth.auth = true;
                //Достаем данные пользователя в случае успеха
                GetUserInfo(login.Text);
                MessageBox.Show("Авторизация прошла успешно!");
                this.Hide();


                //Закрываем форму
                this.Close();
            }
            else
            {
                //Отобразить сообщение о том, что авторизаия неуспешна
                MessageBox.Show("Неверные данные авторизации!");
            }
        }

       
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Thread myThread1 = new Thread(mnogopotok);
            myThread1.Start();
            myThread1.Abort();
            
            
        }
        

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void login_TextChanged(object sender, EventArgs e)
        {

        }

        private void Pass_TextChanged(object sender, EventArgs e)
        {
            
            
        }
    }
  }

