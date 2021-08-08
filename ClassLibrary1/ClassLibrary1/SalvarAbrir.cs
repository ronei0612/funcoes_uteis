using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Data;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using System.Security.Cryptography;

namespace ClassLibrary1
{
    public class SalvarAbrir
    {
        static public string localVar;
        static public void Salvar(string local, string arquivo, string conteudo)
        {
            if (arquivo != "" && local != "")
            {
                local = local.Replace(@"\", "/");
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);
                if (!File.Exists(local + "/" + arquivo))
                    File.Create(local + "/" + arquivo);
                StreamWriter writer = new StreamWriter(local + "/" + arquivo);
                writer.WriteLine(conteudo);
                writer.Close();
                localVar = conteudo;
            }
        }

        static public void Salvar(string arquivo, string conteudo)
        {
            string local;
            if (arquivo != "")
            {
                local = Path.GetDirectoryName(arquivo);
                arquivo = Path.GetFileName(arquivo);
                local = local.Replace(@"\", "/");
                if (!Directory.Exists(local))
                    Directory.CreateDirectory(local);
                if (!File.Exists(local + "/" + arquivo))
                    File.Create(local + "/" + arquivo);
                StreamWriter writer = new StreamWriter(local + "/" + arquivo);
                writer.WriteLine(conteudo);
                writer.Close();
                //localVar = conteudo;
            }
        }

        static public void Abrir(string arquivo)
        {
            if (arquivo != "")
            {
                arquivo = arquivo.Replace(@"\", "/");
                StreamReader reader = new StreamReader(arquivo);
                string conteudo = reader.ReadToEnd();
                reader.Close();
                localVar = conteudo;
            }
        }

        static public void Abrir(string local, string arquivo)
        {
            if (arquivo != "" && local != "")
            {
                local = local.Replace(@"\", "/");
                StreamReader reader = new StreamReader(local + "/" + arquivo);
                string conteudo = reader.ReadToEnd();
                reader.Close();
                localVar = conteudo;
            }
        }
    }
    public class Xml
    {
        static public string valor;
        //static public bool existe;

        public static string Pegar(string arquivo, string palavra)
        {
            string xml = arquivo;
            string frase = palavra;
            if (File.Exists(xml))
            {
                try
                {
                    var linha = System.IO.File.ReadAllLines(xml).Where(i => i.Contains(frase)).ToList();
                    var result = System.IO.File.ReadAllLines(xml).SkipWhile(x => x != linha[0]).Skip(1).ToList();
                    if (result[0].Contains(">") == true)
                    {
                        var match = Regex.Match(result[0], ">(.*?)</").Groups[1].Value;
                        while (match.Contains(">") == true)
                            match = match.Split('>').Last();
                        valor = match;
                    }
                    else
                    {
                        valor = "Erro procurando tags";
                    }
                }
                catch
                {
                    valor = "Palavra não encontrada";
                }
            }
            return valor;
        }
        public static string Criar(string arquivo, params string[] tags)
        {
            var caminho = Environment.ExpandEnvironmentVariables(arquivo);
            try
            {
                using (DataSet dsResultado = new DataSet())
                {
                    dsResultado.ReadXml(arquivo);
                    XmlTextWriter writer = new XmlTextWriter(arquivo, System.Text.Encoding.UTF8);
                    writer.WriteStartDocument(true);
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 2;
                    writer.WriteStartElement("Principal");
                    foreach (string tag in tags)
                    {
                        if (tag.Contains("=") == true)
                        {
                            var row = tag.Split('=')[0];
                            writer.WriteStartElement(row);
                        }
                        else
                            writer.WriteStartElement(tag);
                        writer.WriteString("a");
                        writer.WriteEndElement();
                    }
                    /*
                    foreach (string tag in tags)
                    {
                        writer.WriteStartElement(tag);
                        writer.WriteString("a");
                        writer.WriteEndElement();
                    }*/
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                    writer.Close();
                }
            }
            catch (Exception ex)
            {
                valor = "Criar: " + ex.Message;
            }
            return valor;
        }

        /*
        public static string Inserirr(string arquivo, params string[] tags)
        {
            var caminho = Environment.ExpandEnvironmentVariables(arquivo);
            try
            {
                using (DataSet dsResultado = new DataSet())
                {
                    dsResultado.ReadXml(caminho);
                    if (dsResultado.Tables.Count == 0)
                    {
                        Criar(arquivo, tags);
                    }
                        //inclui os dados no DataSet
                        dsResultado.Tables[0].Rows.Add(dsResultado.Tables[0].NewRow());
                        foreach (string tag in tags)
                        {
                            var conteudo = tag.Split('=')[1];
                            var row = tag.Split('=')[0];
                            dsResultado.Tables[0].Rows[dsResultado.Tables[0].Rows.Count - 1][row] = conteudo;
                        }
                        dsResultado.AcceptChanges();
                        //--  Escreve para o arquivo XML final usando o método Write
                        dsResultado.WriteXml(caminho, XmlWriteMode.IgnoreSchema);
                }
            }
            catch (Exception ex)
            {
                valor = "Inserir: " + ex.Message;
                Criar(arquivo, tags);
            }
            return valor;
        }*/

        public static string Atualizar(string banco, string tabela, params string[] tag_valor)
        {
            // VERIFICA SE TEM O BANCO
            if (!File.Exists(banco))
            {
                File.Create(banco);
                valor = valor + "criando banco ";
            }

            // VERIFICA SE TEM A TABELA
            if (!File.ReadLines(banco).Contains("<" + tabela + ">"))
            {
                ManipularTxt.SetFinalTexto(banco, "<" + tabela + ">" + "\n</" + tabela + ">");
                valor = valor + "criando tabela ";
            }

            for (int i = 0; i < tag_valor.Length; i++)
            {
                string tag = tag_valor[i];
                i++;
                string valorr = tag_valor[i];
                //Inserir(banco, tabela, tag, valorr);
                // VERIFICA SE TEM ALGUMA TAG
                if (!ManipularTxt.GetEntre(banco, "<" + tabela + ">", "</" + tabela + ">").Contains("<"))
                {
                    ClassLibrary1.ManipularTxt.SetApos(banco, "<" + tabela + ">", "\n\t<" + tag + ">" + "</" + tag + ">");
                    valor = valor + "criando tag ";
                }

                // VERFICA SE TEM A TAG
                if (!ManipularTxt.GetEntre(banco, "<" + tabela + ">", "</" + tabela + ">").Contains("<" + tag + ">"))
                {
                    ClassLibrary1.ManipularTxt.SetApos(banco, "<" + tabela + ">", "\n\t<" + tag + ">" + "</" + tag + ">");
                    valor = valor + "criando tag ";
                }

                // PEGA NUMERO DA LINHA DA TAG
                int startLine = Convert.ToInt16(ClassLibrary1.ManipularTxt.NumeroLinhaContem(banco, "<" + tabela + ">"));
                int lineCount = Convert.ToInt16(ClassLibrary1.ManipularTxt.NumeroLinhaContem(banco, "</" + tabela + ">"));
                var fileLines = System.IO.File.ReadAllLines(banco).Skip((startLine - 1)).Take(lineCount);
                foreach (string j in fileLines)
                {
                    if (j.Contains("<" + tag + ">"))
                    {
                        lineCount = startLine;
                        break;
                    }
                    else
                        startLine++;
                }
                //string valorr1 = Convert.ToString(lineCount);
                // VERIFICA SE O VALOR É O MESMO
                //valor = ManipularTxt.GetLinhaNumero(banco, lineCount);
                if (!ManipularTxt.GetLinhaNumero(banco, lineCount).Contains("<" + tag + ">" + valorr + "</" + tag + ">"))
                {
                    string linhaTag = ClassLibrary1.ManipularTxt.GetLinhaNumero(banco, lineCount);
                    string tagValor = ClassLibrary1.ManipularTxt.GetLinhaNumero(banco, lineCount);
                    string valorTag = ClassLibrary1.ManipularTxt.GetEntre(banco, "<" + tag + ">", "</" + tag + ">");
                    valorTag = "<" + tag + ">" + valorTag + "</" + tag + ">";
                    linhaTag = linhaTag.Replace(valorTag, "<" + tag + ">" + valorr + "</" + tag + ">");
                    ClassLibrary1.ManipularTxt.SubstituirLinhaNumero(banco, lineCount, linhaTag);
                    /*
                    string linhaTag = ManipularTxt.GetLinhaNumero(banco, lineCount);
                    ManipularTxt.SubstituirLinhaNumero(banco, lineCount, linhaTag + "<" + tag + ">" + valorr + "</" + tag + ">");
                    valor = "<" + tag + ">" + valorr + "</" + tag + ">";*/
                }
                valor = "<" + tag + ">" + valorr + "</" + tag + ">" + lineCount;
            }
            return valor;
        }

        public static string Inserir(string banco, string tabela, string tag, string valorr)
        {
            // VERIFICA SE TEM O BANCO
            if (!File.Exists(banco))
            {
                File.Create(banco);
                valor = valor + "criando banco ";
            }

            // VERIFICA SE TEM A TABELA
            if (!File.ReadLines(banco).Contains("<" + tabela + ">"))
            {
                ManipularTxt.SetFinalTexto(banco, "<" + tabela + ">" + "\n</" + tabela + ">");
                valor = valor + "criando tabela ";
            }
            
            // VERIFICA SE TEM A TAG
            if (!ManipularTxt.GetEntre(banco, "<" + tabela + ">", "</" + tabela + ">").Contains("<" + tag + ">"))
            {
                ClassLibrary1.ManipularTxt.SetApos(banco, "<" + tabela + ">", "\n\t<" + tag + ">" + "</" + tag + ">");
                valor = valor + "criando tag ";
            }
            
            int startLine = Convert.ToInt16(ClassLibrary1.ManipularTxt.NumeroLinhaContem(banco, "<" + tabela + ">"));
            int lineCount = Convert.ToInt16(ClassLibrary1.ManipularTxt.NumeroLinhaContem(banco, "</" + tabela + ">"));
            var fileLines = System.IO.File.ReadAllLines(banco).Skip((startLine - 1)).Take(lineCount);
            foreach (string i in fileLines)
            {
                if (i.Contains("<" + tag + ">"))
                {
                    lineCount = startLine;
                    break;
                }
                else
                    startLine++;
            }
            //string valorr1 = Convert.ToString(lineCount);
            if (!ManipularTxt.GetLinhaNumero(banco, lineCount).Contains("\t<" + tag + ">" + valorr + "</" + tag + ">"))
            {
                ManipularTxt.SubstituirLinhaNumero(banco, lineCount, "\t<" + tag + ">" + valorr + "</" + tag + ">");
                valor = "troca valor"+lineCount+valorr;
            }
            return valor;
        }
    }

    public class PdfToText
    {
        public static void Converter(string pdf, string txt)
        {
            string local = pdf;
            string texto = txt;
            using (StreamWriter sw = new StreamWriter(texto))
            {
                sw.WriteLine(parseUsingPDFBox(local));
            }
        }

		private static string parseUsingPDFBox(string input)
		{
		    PDDocument doc = null;
            try
            {
                doc = PDDocument.load(input);
                PDFTextStripper stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
		}
    }
    public class ManipularTxt
    {
        static public string valor;
        static private bool boleano;
        public static string GetEntre (string arquivo, string texto1, string texto2)
        {   
            //valor = Regex.Match(File.ReadAllText(arquivo), @"key" + texto1 + "(.+?)" + texto2).Groups[1].Value;
            //valor = Regex.Match(File.ReadAllText(arquivo), @"key : (.+?)-").Groups[1].Value;
            if (!Existe(arquivo, texto1) && !Existe(arquivo, texto2))
                valor = "nao existe texto1 nem texto2";
            else if (!Existe(arquivo, texto1))
                valor = "nao existe texto1";
            else if (!Existe(arquivo, texto2))
                valor = "nao existe texto2";
            else
            {
                string text = File.ReadAllText(arquivo);
                int pFrom = text.IndexOf(texto1) + texto1.Length;
                valor = text.Substring(pFrom, text.LastIndexOf(texto2) - pFrom);
            }
            return valor;
        }

        public static void Substituir(string arquivo, string substituir, string texto)
        {
            string text = File.ReadAllText(arquivo);
            if (Existe(arquivo, substituir))
                text.Replace(substituir, texto);
        }

        public static string EscreverTemporario(string escrever)
        {
            //string tempFile = System.IO.Path.GetTempFileName();
            string tempFile = Environment.ExpandEnvironmentVariables(@"%temp%\temp123.txt");
            //string tempFile = "";
            System.IO.File.WriteAllText(tempFile, escrever);
            return tempFile;
        }

        public static string GetLinhaAbaixo(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                var text = File.ReadAllLines(arquivo);
                var linhaTabela = text.Where(i => i.Contains(texto)).ToList();
                var linhaTag = text.SkipWhile(x => x != linhaTabela[0]).Skip(1).ToList();
                valor = linhaTag[0];
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static string SetApos(string arquivo, string apos, string texto)
        {
            if (Existe(arquivo, apos))
            {
                string text = File.ReadAllText(arquivo);
                string insertPoint = apos;
                int index = text.IndexOf(insertPoint) + insertPoint.Length;
                text = text.Insert(index, texto);
                File.WriteAllText(arquivo, text);
            }
            else
                valor = "texto apos nao existe";
            return valor;
        }

        public static string GetApos(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                string text = File.ReadAllText(arquivo);
                int pFrom = text.IndexOf(texto) + texto.Length;
                valor = new StringReader(text.Substring(text.IndexOf(texto) + texto.Length)).ReadLine();
            }
            else
                valor = "texto apos nao existe";
            return valor;
        }

        public static void SetFinalTexto(string arquivo, string texto)
        {
            File.AppendAllText(arquivo, texto);
        }

        public static string GetLinhaNumero(string arquivo, int numero)
        {
            numero = --numero;
            valor = File.ReadLines(arquivo).Skip(numero).Take(1).First();
            return valor;
        }

        public static string LinhaContem(string arquivo, string texto, int numeroLinha)
        {
            if (Existe(arquivo, texto))
            {
                var text = File.ReadAllLines(arquivo);
                var linha = text.Where(i => i.Contains(texto)).ToList();
                if (numeroLinha == 0)
                    foreach (string i in linha)
                        valor = valor + i + Environment.NewLine;
                else
                {
                    numeroLinha--;
                    valor = linha[numeroLinha];
                }
            }
            return valor;
        }

        public static string ExcluirLinha(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                string line = null;
                string line_to_delete = texto;
                string tempFile = System.IO.Path.GetTempFileName();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(arquivo))
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(tempFile))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (String.Compare(line, line_to_delete) == 0)
                                continue;
                            writer.WriteLine(line);
                        }
                    }
                }
                System.IO.File.Delete(arquivo);
                System.IO.File.Move(tempFile, arquivo);
            }
            return valor;
        }

        public static string ExcluirLinhaNumero(string arquivo, int linha)
        {
            string tempFile = System.IO.Path.GetTempFileName();
            string line = null;
            int line_number = 0;
            using (StreamReader reader = new StreamReader(arquivo))
            {
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line_number++;
                        if (line_number == linha)
                            continue;
                        writer.WriteLine(line);
                    }
                }
            }
            System.IO.File.Delete(arquivo);
            System.IO.File.Move(tempFile, arquivo);
            valor = File.ReadAllText(arquivo);
            return valor;
        }

        public static string ExcluirLinhaContem(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                var text = File.ReadAllLines(arquivo);
                var linha = text.Where(i => i.Contains(texto)).ToList();
                foreach (string i in linha)
                    ExcluirLinha(arquivo, i);
                valor = File.ReadAllText(arquivo);
            }
            return valor;
        }

        public static string LinhaContemSomente(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                string line;
                using (StreamReader file = new StreamReader(arquivo))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains(texto))
                        {
                            if (line == texto)
                                valor = line;
                        }
                    }
                }
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static string NumeroLinhaContemSomente(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                foreach (var match in File.ReadLines(arquivo).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(texto)))
                {
                    if (match.text == texto)
                        valor = Convert.ToString(match.lineNumber);
                }
                if (valor == null)
                    valor = "texto somente não existe";
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static string NumeroLinhaContem(string arquivo, string texto)
        {
            if (Existe(arquivo, texto))
            {
                foreach (var match in File.ReadLines(arquivo).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(texto)))
                {
                    valor = Convert.ToString(match.lineNumber);
                }
                if (valor == null)
                    valor = "texto somente não existe";
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static void SubstituirLinhaNumero(string arquivo, int linha, string texto)
        {
            string[] arrLine = File.ReadAllLines(arquivo);
            arrLine[linha - 1] = texto;
            File.WriteAllLines(arquivo, arrLine);
        }

        public static string LinhaAposContemSomente(string arquivo, string texto)
        {
            string num = NumeroLinhaContemSomente(arquivo, texto);
            if (num == "texto somente não existe")
                valor = "texto somente não existe";
            else
            {
                int numero = Convert.ToInt32(num);
                valor = File.ReadLines(arquivo).Skip(numero).Take(1).First();
            }
            return valor;
        }

        public static bool Existe(string arquivo, string texto)
        {
            boleano = File.ReadAllText(arquivo).Contains(texto);
            return boleano;
        }

        // Criptografar arquivo
        public static void CriptArquivo(string filePath, string key)
        {
            byte[] data = File.ReadAllBytes(filePath);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider()
                {
                    Key = keys,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
                {
                    ICryptoTransform transform = tripDes.CreateEncryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    File.WriteAllBytes(filePath, results.ToArray());
                }
            }
        }

        //Desencriptografar arquivo
        public static void DescriptArquivo(string filePath, string key)
        {
            byte[] data = File.ReadAllBytes(filePath);
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] keys = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
                using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider()
                {
                    Key = keys,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
                {
                    ICryptoTransform transform = tripDes.CreateDecryptor();
                    byte[] results = transform.TransformFinalBlock(data, 0, data.Length);
                    File.WriteAllBytes(filePath, results.ToArray());
                }
            }
        }
    }
    public class ManipularString
    {
        static public string valor;
        static private bool boleano;
        public static string GetEntre(string textoTotal, string texto1, string texto2)
        {
            if (!Existe(textoTotal, texto1) && !Existe(textoTotal, texto2))
                valor = "nao existe texto1 nem texto2";
            else if (!Existe(textoTotal, texto1))
                valor = "nao existe texto1";
            else if (!Existe(textoTotal, texto2))
                valor = "nao existe texto2";
            else
            {
                string text = File.ReadAllText(textoTotal);
                int pFrom = text.IndexOf(texto1) + texto1.Length;
                valor = text.Substring(pFrom, text.LastIndexOf(texto2) - pFrom);
            }
            return valor;
        }

        public static void Substituir(string textoTotal, string substituir, string texto)
        {
            textoTotal.Replace(substituir, texto);
        }

        public static string GetLinhaAbaixo(string textoTotal, string texto)
        {
            var text = File.ReadAllLines(textoTotal);
            var linhaTabela = text.Where(i => i.Contains(texto)).ToList();
            var linhaTag = text.SkipWhile(x => x != linhaTabela[0]).Skip(1).ToList();
            valor = linhaTag[0];
            return valor;
        }

        public static string SetApos(string textoTotal, string apos, string texto)
        {
            string text = File.ReadAllText(textoTotal);
            string insertPoint = apos;
            int index = text.IndexOf(insertPoint) + insertPoint.Length;
            text = text.Insert(index, texto);
            File.WriteAllText(textoTotal, text);
            return valor;
        }

        public static string GetApos(string textoTotal, string texto)
        {
            if (Existe(textoTotal, texto))
            {
                string text = File.ReadAllText(textoTotal);
                int pFrom = text.IndexOf(texto) + texto.Length;
                valor = new StringReader(text.Substring(text.IndexOf(texto) + texto.Length)).ReadLine();
            }
            else
                valor = "texto apos nao existe";
            return valor;
        }

        public static void SetFinalTexto(string textoTotal, string texto)
        {
            File.AppendAllText(textoTotal, texto);
        }

        public static string GetLinhaNumero(string textoTotal, int numero)
        {
            numero = --numero;
            valor = File.ReadLines(textoTotal).Skip(numero).Take(1).First();
            return valor;
        }

        public static string LinhaContem(string textoTotal, string texto, int numeroLinha)
        {
            if (Existe(textoTotal, texto))
            {
                var text = File.ReadAllLines(textoTotal);
                var linha = text.Where(i => i.Contains(texto)).ToList();
                if (numeroLinha == 0)
                    foreach (string i in linha)
                        valor = valor + i + Environment.NewLine;
                else
                {
                    numeroLinha--;
                    valor = linha[numeroLinha];
                }
            }
            return valor;
        }

        public static string ExcluirLinha(string textoTotal, string texto)
        {
            if (Existe(textoTotal, texto))
            {
                string line = null;
                string line_to_delete = texto;
                string tempFile = System.IO.Path.GetTempFileName();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(textoTotal))
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(tempFile))
                    {
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (String.Compare(line, line_to_delete) == 0)
                                continue;
                            writer.WriteLine(line);
                        }
                    }
                }
                System.IO.File.Delete(textoTotal);
                System.IO.File.Move(tempFile, textoTotal);
            }
            return valor;
        }

        public static string ExcluirLinhaNumero(string textoTotal, int linha)
        {
            string tempFile = System.IO.Path.GetTempFileName();
            string line = null;
            int line_number = 0;
            using (StreamReader reader = new StreamReader(textoTotal))
            {
                using (StreamWriter writer = new StreamWriter(tempFile))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        line_number++;
                        if (line_number == linha)
                            continue;
                        writer.WriteLine(line);
                    }
                }
            }
            System.IO.File.Delete(textoTotal);
            System.IO.File.Move(tempFile, textoTotal);
            valor = File.ReadAllText(textoTotal);
            return valor;
        }

        /*
        public static string ExcluirLinhaContem(string textoTotal, string texto)
        {
            var text = File.ReadAllLines(textoTotal);
            //var linha = text.Where(i => i.Contains(texto)).ToList();
            var linha = textoTotal.Where(textoTotal.Contains(texto)).ToList();
            foreach (string i in linha)
                ExcluirLinha(textoTotal, i);
            valor = File.ReadAllText(textoTotal);
            return valor;
        }*/

        public static string LinhaContemSomente(string textoTotal, string texto)
        {
            if (Existe(textoTotal, texto))
            {
                string line;
                using (StreamReader file = new StreamReader(textoTotal))
                {
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Contains(texto))
                        {
                            if (line == texto)
                                valor = line;
                        }
                    }
                }
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static string NumeroLinhaContemSomente(string textoTotal, string texto)
        {
            if (Existe(textoTotal, texto))
            {
                foreach (var match in File.ReadLines(textoTotal).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(texto)))
                {
                    if (match.text == texto)
                        valor = Convert.ToString(match.lineNumber);
                }
                if (valor == null)
                    valor = "texto somente não existe";
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static string NumeroLinhaContem(string textoTotal, string texto)
        {
            if (Existe(textoTotal, texto))
            {
                foreach (var match in File.ReadLines(textoTotal).Select((text, index) => new { text, lineNumber = index + 1 }).Where(x => x.text.Contains(texto)))
                {
                    valor = Convert.ToString(match.lineNumber);
                }
                if (valor == null)
                    valor = "texto somente não existe";
            }
            else
                valor = "texto nao existe";
            return valor;
        }

        public static void SubstituirLinhaNumero(string textoTotal, int linha, string texto)
        {
            string[] arrLine = File.ReadAllLines(textoTotal);
            arrLine[linha - 1] = texto;
            File.WriteAllLines(textoTotal, arrLine);
        }

        public static string LinhaAposContemSomente(string textoTotal, string texto)
        {
            string num = NumeroLinhaContemSomente(textoTotal, texto);
            if (num == "texto somente não existe")
                valor = "texto somente não existe";
            else
            {
                int numero = Convert.ToInt32(num);
                valor = File.ReadLines(textoTotal).Skip(numero).Take(1).First();
            }
            return valor;
        }

        public static bool Existe(string textoTotal, string texto)
        {
            boleano = File.ReadAllText(textoTotal).Contains(texto);
            return boleano;
        }
    }
}
/*  adicionar Row em DATAGRIDVIEW
string[] colunas = new string[] { };
string[] valores = new string[] { };
DataTable Dt = new DataTable();
DataRow dr = Dt.NewRow();
int num = 0;
foreach (string coluna in colunas)
{
    Dt.Columns.Add(coluna);
    dr[num] = valores[num];
    num++;
}
Dt.Rows.Add(dr);
dataGridView1.DataSource = Dt;*/
