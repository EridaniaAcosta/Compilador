using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;

namespace Compilador
{
    public partial class Compilador : Form
    {
        public Compilador()
        {
            InitializeComponent();
        }

        string unir_cad = "";
        string espacio = "[' ']";
        string salto = "['\n']";

        string unir_string = "";
        string unir_com = "";

        char validar_uno_mas = '0';
        int contador_error_compilar = 0;

        int recorrido = 0;
        string[] lexi_a_sint = null;
        string[] num_linea = null;
        string[] num_columna = null;

        string[] variables = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        int contador_variables = 0;
        string[] tipo_variables = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        string inicio_correcto = "0";
        string proceso_correcto = "0";
        string ya_imprimio_no_hay_proc = "0";

        string proceso_hay = "0";
        string fin_hay = "0";

        string[] pasar_a_c = null;
        int pasos_pasar_a_c = 0;

        int recorrido_sum = 0;

        private void BLTabla_Click(object sender, EventArgs e)
        {
            //Limpiar el Datagrid             
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            dataGridView4.Rows.Clear();
            //Limpiar el TextBoxt             
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.Focus();
        }

        private void BSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string publico = textBox1.Text.Replace("publico", "public");
            string mainvar = publico.Replace("mi_main", "main");
            string impresion = mainvar.Replace("imprimir", "MessageBox.Show");
            string entero1 = impresion.Replace("entero", "int");
            string entero2 = entero1.Replace("integer", "int");
            string simb = entero2.Replace("->", "=");
            string cond1 = simb.Replace("si", "if");
            string cond2 = cond1.Replace("mientras", "while");
            string cond3 = cond2.Replace("hacer", "do");
            string cadena1 = cond3.Replace("cadena", "string");

            textBox2.Text = cadena1;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            this.Analizar();

            int contador_errores = 0;
            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                if ((dataGridView1.Rows[x].Cells[1].Value.ToString()).Equals("ERROR"))
                {
                    contador_errores += 1;
                    dataGridView1.Rows[x].DefaultCellStyle.BackColor = Color.Pink;

                }
            }

            for (int x = 0; x < pasos_pasar_a_c; x++)
            {
                pasar_a_c[x] = "";
            }
            pasos_pasar_a_c = 0;
            dataGridView2.Rows.Clear();
            for (int i = 0; i < variables.Length; i++)
            {
                variables[i] = "0";
            }
            inicio_correcto = "0";
            proceso_correcto = "0";
            proceso_hay = "0";
            fin_hay = "0";
            contador_error_compilar = 0;
            contador_variables = 0;
            this.Sintactico();

            for (int x = 0; x < dataGridView2.RowCount; x++)
            {
                if ((dataGridView2.Rows[x].Cells[0].Value.ToString()).Equals("ERROR"))
                {

                    dataGridView2.Rows[x].DefaultCellStyle.BackColor = Color.Pink;
                    contador_error_compilar += 1;
                }
            }

            if (contador_error_compilar == 0)
            {
                using (StreamWriter writer = new StreamWriter("C:\\Users\\qjmso\\Desktop\\2-2021\\compiladores\\Compilador\\Compilador\\DOOES en C++.txt", false))

                for (int x = 0; x < pasos_pasar_a_c; x++)
                {
                    if (x == 0)
                    {
                        writer.WriteLine("#include <iostream>");
                        writer.WriteLine("using namespace std;");
                        writer.WriteLine("int main() {");
                        writer.WriteLine("");
                        writer.WriteLine("");
                        writer.WriteLine("");
                    }
                    
                    writer.WriteLine(pasar_a_c[x].ToString());

                    if (x == pasos_pasar_a_c - 1)
                    {
                        writer.WriteLine("");
                        writer.WriteLine("");
                        writer.WriteLine("return 0;");
                        writer.WriteLine("}");
                    }
                }
            }
        }

        public void Analizar()
        {
            dataGridView1.Rows.Clear();

            char validar_com = '0';
            char validar_cad_string = '0';

            string comentario = "@";
            string cad_string = "[\"]";
            string texto = textBox1.Text;

            foreach (char letra in texto)
            {
                string letra2 = letra.ToString();

                if (Regex.IsMatch(letra2, cad_string))
                {
                    if (validar_cad_string.Equals('0'))
                    {
                        validar_cad_string = '1';
                    }
                    else
                    {
                        dataGridView1.Rows.Add(unir_string + "\"", "string");
                        validar_cad_string = '0';
                        unir_string = "";
                    }
                }

                if (validar_cad_string.Equals('1'))
                {
                    unir_string = unir_string + letra2;
                }

                if (Regex.IsMatch(letra2, comentario))
                {
                    validar_com = '1';
                }

                if (validar_com.Equals('1'))
                {
                    unir_com = unir_com + letra2;

                    if (letra.Equals('\n'))
                    {
                        dataGridView1.Rows.Add(unir_com + "", "COMENTARIO");
                        validar_com = '0';
                        unir_com = "";
                    }
                }

                else if (validar_com.Equals('0') & validar_cad_string.Equals('0') & letra2 != "\"" & letra2 != "\r")
                {
                    if (letra2 == " " || letra2 == "\n")
                    {
                        this.AnalizarPalabras();
                    }
                    else
                    {
                        unir_cad = unir_cad + letra2;
                    }
                }
            }
        }

        public void AnalizarPalabras()
        {
            string exp_minusculas = "[A-Z]+";

            if (Regex.IsMatch(unir_cad, exp_minusculas))
            {
                dataGridView1.Rows.Add(unir_cad + "", "ERROR");
                unir_cad = "";
            }
            else
            {
                this.VerificarLexema();
            }
        }

        public void VerificarLexema()
        {
            string[] reservado = { "si", "ver", "mientras", "entero", "cadena" };

            string exp_numeros = "^[0-9]+$[0-9]?";
            string exp_delimitador = "^[;|(|)|{|}]$";
            string exp_operadores = "^[+|-|/|*]$";
            string asignacion = "^[#|=]$";
            string exp_comparador = "^[<|>]$|^==$";
            string variable = "^[a-z]+$[a-z]?";

            char validar_reservado = '0';

            for (int i = 0; i < 5; i++)
            {
                if (unir_cad.Equals(reservado[i]))
                {
                    dataGridView1.Rows.Add(unir_cad + "", "RESERVADO");
                    validar_reservado = '1';
                    if (Regex.IsMatch(unir_cad, "si"))
                    {
                        validar_uno_mas = '1';
                    }
                }
            }

            byte[] bytes = Encoding.ASCII.GetBytes(unir_cad);

            if (Regex.IsMatch(unir_cad, exp_numeros))
            {
                dataGridView1.Rows.Add(unir_cad + "", "NUMERO");

                //bytes = Encoding.ASCII.GetBytes(unir_cad);
                //foreach (byte b in bytes)
                //{
                //    dataGridView4.Rows.Add(b, unir_cad);
                //}
            }
            else if (Regex.IsMatch(unir_cad, exp_delimitador))
            {
                dataGridView1.Rows.Add(unir_cad + "", "DELIMITADOR");

                bytes = Encoding.ASCII.GetBytes(unir_cad);
                foreach (byte b in bytes)
                {
                    dataGridView4.Rows.Add(b, unir_cad);
                }
            }
            else if (Regex.IsMatch(unir_cad, exp_operadores))
            {
                dataGridView1.Rows.Add(unir_cad + "", "OPERADOR");

                bytes = Encoding.ASCII.GetBytes(unir_cad);
                foreach (byte b in bytes)
                {
                    dataGridView4.Rows.Add(b, unir_cad);
                }
            }
            else if (Regex.IsMatch(unir_cad, asignacion))
            {
                dataGridView1.Rows.Add(unir_cad + "", "ASIGNACION");
                dataGridView2.Rows.Add("Se ha detectado setencia de asignacion");

                bytes = Encoding.ASCII.GetBytes(unir_cad);
                foreach (byte b in bytes)
                {
                    dataGridView4.Rows.Add(b, unir_cad);
                }
            }
            else if (Regex.IsMatch(unir_cad, exp_comparador))
            {
                dataGridView1.Rows.Add(unir_cad + "", "COMPARADOR");

                bytes = Encoding.ASCII.GetBytes(unir_cad);
                foreach (byte b in bytes)
                {
                    dataGridView4.Rows.Add(b, unir_cad);
                }
            }
            else if (Regex.IsMatch(unir_cad, variable))
            {
                dataGridView1.Rows.Add(unir_cad + "", "VARIABLE");

                //bytes = Encoding.ASCII.GetBytes(unir_cad);
                //foreach (byte b in bytes)
                //{
                //    dataGridView4.Rows.Add(b, unir_cad);
                //}
            }            
            else if (validar_reservado.Equals('0') & unir_cad != "" & unir_cad != "\"")
            {
                dataGridView1.Rows.Add(unir_cad + "", "ERROR");
            }
            unir_cad = "";
        }

        public void Sintactico()
        {
            pasar_a_c = new string[30];

            int contador_comentarios = 0;
            int contador_comentarios_fila_menos = 0;

            for (int x = 0; x < dataGridView1.RowCount; x++)
            {
                if ((dataGridView1.Rows[x].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                {
                    contador_comentarios_fila_menos += 1;
                }
            }

            if (contador_comentarios_fila_menos > 0)
            {
                lexi_a_sint = new string[(dataGridView1.RowCount - contador_comentarios_fila_menos) + 1];

                for (int s = 0; s <= (dataGridView1.RowCount - 1); s++)
                {
                    if ((dataGridView1.Rows[s].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                    {
                        contador_comentarios += 1;
                    }
                    else
                    {
                        lexi_a_sint[s - contador_comentarios] = dataGridView1.Rows[s].Cells[0].Value.ToString();
                        num_linea[s - contador_comentarios] = dataGridView1.Rows[s].Cells[2].Value.ToString();
                        num_columna[s - contador_comentarios] = dataGridView1.Rows[s].Cells[3].Value.ToString();
                    }
                }
            }
            else
            {
                lexi_a_sint = new string[(dataGridView1.RowCount - contador_comentarios_fila_menos) + 1];

                for (int s = 0; s < (dataGridView1.RowCount - contador_comentarios_fila_menos); s++)
                {
                    if ((dataGridView1.Rows[s].Cells[1].Value.ToString()).Equals("COMENTARIO"))
                    {
                        contador_comentarios += 1;
                    }
                    else
                    {
                        lexi_a_sint[s - contador_comentarios] = dataGridView1.Rows[s].Cells[0].Value.ToString();
                    }
                }
            }
            lexi_a_sint[dataGridView1.RowCount - contador_comentarios_fila_menos] = "ultima linea";

            //for (int recorridox = 0; recorridox < lexi_a_sint.Length; recorridox++)
            //{
            //    if (Regex.IsMatch(lexi_a_sint[recorridox], "inicio"))
            //    {
            //        recorridox += 1;
            //        if (Regex.IsMatch(lexi_a_sint[recorridox], ";"))
            //        {
            //            recorridox += 1;
            //        }                    
            //    }

            //    if (Regex.IsMatch(lexi_a_sint[recorridox], "proceso"))
            //    {
            //        recorridox += 1;
            //        if (Regex.IsMatch(lexi_a_sint[recorridox], ";"))
            //        {
            //            recorridox += 1;
            //            proceso_hay = "1";
            //        }

            //    }

            //    if (Regex.IsMatch(lexi_a_sint[recorridox], "fin"))
            //    {
            //        recorridox += 1;
            //        if (Regex.IsMatch(lexi_a_sint[recorridox], ";"))
            //        {
            //            recorridox += 1;
            //            fin_hay = "1";
            //        }

            //    }
            //}

            for (recorrido = 0; recorrido < lexi_a_sint.Length; recorrido++)
            {
                recorrido_sum = recorrido;
                this.estruc_var_cadena();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_var_entera();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_var_cadena();

                recorrido_sum = recorrido;
                this.estruc_var_entera();

                recorrido_sum = recorrido;
                this.estruc_ver();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_si();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_mientras();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_mientrasx();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_ver();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_si();
                recorrido = recorrido_sum;

                recorrido_sum = recorrido;
                this.estruc_mientras();
                recorrido = recorrido_sum;

            }
        }

        public void estruc_var_entera()
        {
            //inicia reconocimento de declaracion de variable entera
            string existe = "no";
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "entero"))
            {
                dataGridView2.Rows.Add("Se ha detectado variable entera");
                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], ";"))
                            {
                                for (int i = 0; i < contador_variables + 1; i++)
                                {
                                    if (variables[i].Equals(lexi_a_sint[recorrido_sum - 3]))
                                    {
                                        existe = "si";
                                    }
                                }
                                if (existe == "si")
                                {
                                    //dataGridView2.Rows.Add("NOMBRE DE VARIABLE YA EXISTE");
                                }
                                if (existe == "no")
                                {
                                    variables[contador_variables] = lexi_a_sint[recorrido_sum - 3];
                                    tipo_variables[contador_variables] = "numero";
                                    contador_variables += 1;
                                    dataGridView2.Rows.Add("ASIGNACION DE VARIABLE ENTERA");

                                    pasar_a_c[pasos_pasar_a_c] = "int " + lexi_a_sint[recorrido_sum - 3] + " = " + lexi_a_sint[recorrido_sum - 1] + ";" + "\n";
                                    pasos_pasar_a_c += 1;
                                }
                            }
                            else
                            {
                                //dataGridView2.Rows.Add("SE ESPERABA PUNTO Y COMA"); 
                                recorrido_sum -= 1;
                            }
                        }
                        else
                        {
                            //dataGridView2.Rows.Add("SE ESPERABA UN NUMERO"); 
                            recorrido_sum -= 1;
                        }
                    }
                    else
                    {
                      //  dataGridView2.Rows.Add("SE ESPERABA ASIGNADOR"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA VARIABLE"); 
                    recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }
        }

        public void estruc_var_cadena()
        {
            string existe = "no";
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "cadena"))
            {
                dataGridView2.Rows.Add("Se ha detectado una variable de cadena");
                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "#"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                        {
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], ";"))
                            {
                                for (int i = 0; i < contador_variables + 1; i++)
                                {
                                    if (variables[i].Equals(lexi_a_sint[recorrido_sum - 3]))
                                    {
                                        existe = "si";
                                    }
                                }
                                if (existe == "si")
                                {
                                 //   dataGridView2.Rows.Add("NOMBRE DE VARIABLE YA EXISTE");
                                }
                                if (existe == "no")
                                {
                                    variables[contador_variables] = lexi_a_sint[recorrido_sum - 3];
                                    tipo_variables[contador_variables] = "cadena";
                                    contador_variables += 1;
                                    dataGridView2.Rows.Add("ASIGNACION DE VARIABLE CADENA");
                                    pasar_a_c[pasos_pasar_a_c] = "string " + lexi_a_sint[recorrido_sum - 3] + " = " + lexi_a_sint[recorrido_sum - 1] + ";" + "\n";
                                    pasos_pasar_a_c += 1;
                                }
                            }
                            else
                            {
                                //dataGridView2.Rows.Add("SE ESPERABA PUNTO Y COMA"); 
                                recorrido_sum -= 1;
                            }
                        }
                        else
                        {
                            //dataGridView2.Rows.Add("SE ESPERABA UNA CADENA"); 
                            recorrido_sum -= 1;
                        }
                    }
                    else
                    {
                        //dataGridView2.Rows.Add("SE ESPERABA ASIGNADOR"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA VARIABLE"); 
                    recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }
        }

        public void estruc_ver()
        {
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "ver"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], ";"))
                    {
                      //  dataGridView2.Rows.Add("IMPRIMIR EN PANTALLA VER");
                        pasar_a_c[pasos_pasar_a_c] = " cout << " + lexi_a_sint[recorrido_sum - 1] + " << endl" + ";" + "\n";
                        pasos_pasar_a_c += 1;
                    }
                    else
                    {
                        // dataGridView2.Rows.Add("SE ESPERABA PUNTO Y COMA"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    // dataGridView2.Rows.Add("SE ESPERABA UNA CADENA"); 
                    recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }
        }

        public void estruc_si()
        {
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "si"))
            {
                dataGridView2.Rows.Add("Se ha detectado el comienzo de una condicion si");
                recorrido_sum += 1;
                if (lexi_a_sint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion();
                    recorrido_sum += 1;
                    if (lexi_a_sint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("SI ( ) { }" + " FINALIZACION ");
                            }
                            else
                            {
                                //dataGridView2.Rows.Add("SE ESPERABA CIERRE DE LLAVE"); 
                                recorrido_sum -= 1;
                            }
                        }
                        else
                        {
                            //dataGridView2.Rows.Add("SE ESPERABA APERTURA DE LLAVE"); 
                            recorrido_sum -= 1;
                        }
                    }
                    else
                    {
                        //dataGridView2.Rows.Add("SE ESPERABA CIERRE DE PARENTESIS"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA APERTURA DE PARENTESIS"); 
                    recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }
        }

        public void estruc_mientras()
        {
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "mientras"))
            {
                dataGridView2.Rows.Add("Se ha detectado una setencia mientras");
                recorrido_sum += 1;
                if (lexi_a_sint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion_mientras();
                    recorrido_sum += 1;
                    if (lexi_a_sint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("MIENTRAS ( ) { }" + " FINALIZACION " );
                            }
                            else
                            {
                                //dataGridView2.Rows.Add("SE ESPERABA CIERRE DE LLAVE"); 
                                recorrido_sum -= 1;
                            }
                        }
                        else
                        {
                            // dataGridView2.Rows.Add("SE ESPERABA APERTURA DE LLAVE"); 
                            recorrido_sum -= 1;
                        }
                    }
                    else
                    {
                        //dataGridView2.Rows.Add("SE ESPERABA CIERRE DE PARENTESIS"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA APERTURA DE PARENTESIS"); 
                    recorrido_sum -= 1;
                }

                recorrido = recorrido_sum;
            }
        }

        public void estruc_mientrasx()
        {
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "mientras"))
            {
                recorrido_sum += 1;
                if (lexi_a_sint[recorrido_sum].Equals("("))
                {
                    recorrido_sum += 1;
                    this.estruc_comparacion_mientras();
                    recorrido_sum += 1;
                    if (lexi_a_sint[recorrido_sum].Equals(")"))
                    {
                        recorrido_sum += 1;
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "{"))
                        {
                            recorrido_sum += 1;
                            this.estruc_ver_dentro_de_si();
                            recorrido_sum += 1;
                            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "}"))
                            {
                                dataGridView2.Rows.Add("MIENTRAS ( ) { }");
                            }
                            else
                            {
                      //          dataGridView2.Rows.Add("SE ESPERABA CIERRE DE LLAVE"); 
                                recorrido_sum -= 1;
                            }
                        }
                        else
                        {
                        //    dataGridView2.Rows.Add("SE ESPERABA APERTURA DE LLAVE"); 
                            recorrido_sum -= 1;
                        }
                    }
                    else
                    {
                       // dataGridView2.Rows.Add("SE ESPERABA CIERRE DE PARENTESIS"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA APERTURA DE PARENTESIS"); 
                    recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }
        }

        public void estruc_ver_dentro_de_si()
        {
            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "ver"))
            {
                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], ";"))
                    {
                        dataGridView2.Rows.Add("IMPRIMIR EN PANTALLA VER");
                        pasar_a_c[pasos_pasar_a_c] = "{ " + "\n";
                        pasos_pasar_a_c += 1;
                        pasar_a_c[pasos_pasar_a_c] = " cout << " + lexi_a_sint[recorrido_sum - 1] + " << endl" + ";" + "\n";
                        pasos_pasar_a_c += 1;
                        pasar_a_c[pasos_pasar_a_c] = "} " + "\n";
                        pasos_pasar_a_c += 1;
                    }
                    else
                    {
                      //  dataGridView2.Rows.Add("SE ESPERABA PUNTO Y COMA"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA UNA CADENA"); 
                    recorrido_sum -= 1;
                }
                recorrido = recorrido_sum;
            }
            else
            {
                pasar_a_c[pasos_pasar_a_c] = "{ " + "\n";
                pasos_pasar_a_c += 1;

                pasar_a_c[pasos_pasar_a_c] = "} " + "\n";
                pasos_pasar_a_c += 1;
                recorrido_sum -= 1;
            }
        }

        public void estruc_comparacion()
        {
            string existe = "no";
            string existe2 = "no";
            string tipo = "ninguno";
            string tipo2 = "ninguno";

            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
            {
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contador_variables + 1; i++)
                    {
                        if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                        {
                            tipo = tipo_variables[i];
                        }
                    }
                }
                else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[0-9]+$[0-9]?"))
                {
                    tipo = "numero";
                    existe = "si";
                }
                else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                {
                    tipo = "cadena";
                    existe = "si";
                }

                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contador_variables + 1; i++)
                    {
                        if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                        {
                            existe = "si";
                        }
                    }
                    if (existe == "si")
                    {

                    }
                    if (existe == "no")
                    {
                      //  dataGridView2.Rows.Add("NOMBRE DE VARIABLE NO DECLARADA");
                    }
                }

                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[<|>]$|^==$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
                    {
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contador_variables + 1; i++)
                            {
                                if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                                {
                                    tipo2 = tipo_variables[i];
                                }
                            }
                        }
                        else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            tipo2 = "numero";
                            existe2 = "si";
                        }
                        else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                        {
                            tipo2 = "cadena";
                            existe2 = "si";
                        }

                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contador_variables + 1; i++)
                            {
                                if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                                {
                                    existe2 = "si";
                                }
                            }
                            if (existe2 == "no")
                            {
                        //        dataGridView2.Rows.Add("NOMBRE DE VARIABLE NO DECLARADA");
                            }
                        }

                        if (existe == "no" || existe2 == "no")
                        {

                        }
                        else
                        {
                            if (tipo == tipo2)
                            {
                                dataGridView2.Rows.Add(" COMPARACION");
                                pasar_a_c[pasos_pasar_a_c] = "if ( " + lexi_a_sint[recorrido_sum - 2] + " " + lexi_a_sint[recorrido_sum - 1] + " " + lexi_a_sint[recorrido_sum] + " )" + "\n";
                                pasos_pasar_a_c += 1;
                            }
                            else
                            {
                          //      dataGridView2.Rows.Add(" DATOS DIFERENTES EN COMPARACION ");
                            }
                        }
                    }
                    else
                    {
                        //dataGridView2.Rows.Add("SE ESPERABA VARIABLE, CADENA O NUMERO"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                    //dataGridView2.Rows.Add("SE ESPERABA UN COMPARADOR"); 
                    recorrido_sum -= 1;
                }
            }
            else
            {
                //dataGridView2.Rows.Add("SE ESPERABA UNA VARIABLE, CADENA O NUMERO"); 
                recorrido_sum -= 1;
            }
        }

        public void estruc_comparacion_mientras()
        {
            string existe = "no";
            string existe2 = "no";
            string tipo = "ninguno";
            string tipo2 = "ninguno";

            if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
            {
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contador_variables + 1; i++)
                    {
                        if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                        {
                            tipo = tipo_variables[i];
                        }
                    }
                }
                else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[0-9]+$[0-9]?"))
                {
                    tipo = "numero";
                    existe = "si";
                }
                else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                {
                    tipo = "cadena";
                    existe = "si";
                }

                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                {
                    for (int i = 0; i < contador_variables + 1; i++)
                    {
                        if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                        {
                            existe = "si";
                        }
                    }
                    if (existe == "no")
                    {
                      //  dataGridView2.Rows.Add("NOMBRE DE VARIABLE NO DECLARADA");
                    }
                }

                recorrido_sum += 1;
                if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[<|>]$|^==$"))
                {
                    recorrido_sum += 1;
                    if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$|^[0-9]+$[0-9]?|^\".*\"$"))
                    {
                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contador_variables + 1; i++)
                            {
                                if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                                {
                                    tipo2 = tipo_variables[i];
                                }
                            }
                        }
                        else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^[0-9]+$[0-9]?"))
                        {
                            tipo2 = "numero";
                            existe2 = "si";
                        }
                        else if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^\".*\"$"))
                        {
                            tipo2 = "cadena";
                            existe2 = "si";
                        }

                        if (Regex.IsMatch(lexi_a_sint[recorrido_sum], "^var[(0-9)?]$"))
                        {
                            for (int i = 0; i < contador_variables + 1; i++)
                            {
                                if (variables[i].Equals(lexi_a_sint[recorrido_sum]))
                                {
                                    existe2 = "si";
                                }
                            }
                            if (existe2 == "no")
                            {
                                //dataGridView2.Rows.Add("NOMBRE DE VARIABLE NO DECLARADA");
                            }
                        }

                        if (existe == "no" || existe2 == "no")
                        {

                        }
                        else
                        {
                            if (tipo == tipo2)
                            {
                                dataGridView2.Rows.Add(" COMPARACION");
                                pasar_a_c[pasos_pasar_a_c] = "while ( " + lexi_a_sint[recorrido_sum - 2] + " " + lexi_a_sint[recorrido_sum - 1] + " " + lexi_a_sint[recorrido_sum] + " )" + "\n";
                                pasos_pasar_a_c += 1;
                            }
                            else
                            {
                               // dataGridView2.Rows.Add(" DATOS DIFERENTES EN COMPARACION ");
                            }
                        }
                    }
                    else
                    {
                        //dataGridView2.Rows.Add("SE ESPERABA VARIABLE, CADENA O NUMERO"); 
                        recorrido_sum -= 1;
                    }
                }
                else
                {
                   // dataGridView2.Rows.Add("SE ESPERABA UN COMPARADOR"); 
                    recorrido_sum -= 1;
                }
            }
            else
            {
               // dataGridView2.Rows.Add("SE ESPERABA UNA VARIABLE, CADENA O NUMERO"); 
                recorrido_sum -= 1;
            }

        }

        private void Compilador_Load(object sender, EventArgs e)
        {
            //string h = "hi";
            //Console.WriteLine(Encoding.ASCII.GetBytes(h.ToString())[1]);
            //MessageBox.Show("hola");

            //string author = "Mahesh Chand";
            //byte[] bytes = Encoding.ASCII.GetBytes(author);
            

            //foreach (byte b in bytes)
            //{
            //    Console.WriteLine(b);
            //    dataGridView4.Rows.Add(b, "var");
            //    //textBox1.Text = bytes;
            //}
        }
    }
}
