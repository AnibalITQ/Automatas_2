using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
//PROYECTO 1
/*
    Requerimiento 1: Mensajes del printf deben salir sin comillas
                     Incluir \n y \t como secuencias de escape
    Requerimiento 2: Agregar el % al PorFactor
                     Modifcar el valor de una variable con ++,--,+=,-=,*=,/=.%=
    Requerimiento 3: Cada vez que se haga un match(Tipos.Identificador) verficar el
                     uso de la variable
                     Icremento(), Printf(), Factor() y usar getValor y Modifica
                     Levantar una excepcion en scanf() cuando se capture un string
    Requerimiento 4: Implementar la ejecución del ELSE
*/
//PROYECTO 2
/*
Requerimiento 1: Implementar la ejecución del while
Requerimiento 2: Implementar la ejecucion del do-while
Requerimiento 3: Implementar la ejecución del for
Requerimiento 4: Marcar errores semanticos
Requerimiento 5: CAST
*/
//PROYECTO 3 (ASM)
/*
    Requerimiento 1: Programar scanf 
    Requerimiento 2: Programar printf
    Requerimiento 3: Programar ++,--,+=,-=,*=,/=,%=
    Requerimiento 4: Programar else
    Requerimiento 5: Programar do para que gerenre una sola vez el codigo
    Requerimiento 6: Programar while para que gerenre una sola vez el codigo
    Requerimiento 7: Programar el for para que gerenre una sola vez el codigo
    Requerimiento 8: Programar el CAST
*/

namespace Sintaxis_2
{
    public class Lenguaje : Sintaxis
    {
        List<Variable> lista;
        Stack<float> stack;
        int contIf,contFor;

        Variable.TiposDatos tipoDatoExpresion;
        public Lenguaje()
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            Variable.TiposDatos tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf=contFor=1;
        }
        public Lenguaje(string nombre) : base(nombre)
        {
            lista = new List<Variable>();
            stack = new Stack<float>();
            Variable.TiposDatos tipoDatoExpresion = Variable.TiposDatos.Char;
            contIf=contFor=1;
        }

        //Programa  -> Librerias? Variables? Main
        public void Programa()
        {
            asm.WriteLine("include 'emu8086.inc'");
            asm.WriteLine("org 100h");
            if (getContenido() == "#")
            {
                Librerias();
            }
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
            Main(true);
            asm.WriteLine("RET");
            asm.WriteLine("define_clear_screen");
            asm.WriteLine("define_scan_num");
            asm.WriteLine("define_print_num");
            asm.WriteLine("define_print_num");
            Imprime();
        }

        private void Imprime()
        {
            log.WriteLine("-----------------");
            log.WriteLine("V a r i a b l e s");
            log.WriteLine("-----------------");
            asm.WriteLine(";V a r i a b l e s" );
            foreach (Variable v in lista)
            {
                log.WriteLine(v.getNombre() + " " + v.getTiposDatos() + " = " + v.getValor());
                asm.WriteLine(v.getNombre()+" dw 0h");

            }
            log.WriteLine("-----------------");
        }

        private bool Existe(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return true;
                }
            }
            return false;
        }
        private void Modifica(string nombre, float nuevoValor)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    v.setValor(nuevoValor);
                }
            }
        }
        private float getValor(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getValor();
                }
            }
            return 0;
        }
        private Variable.TiposDatos getTipo(string nombre)
        {
            foreach (Variable v in lista)
            {
                if (v.getNombre() == nombre)
                {
                    return v.getTiposDatos();
                }
            }
            return Variable.TiposDatos.Char;
        }
        private Variable.TiposDatos getTipo(float resultado)
        {
            if (resultado % 1 != 0)
            {
                return Variable.TiposDatos.Float;
            }
            else if (resultado < 256)
            {
                return Variable.TiposDatos.Char;
            }
            else if (resultado < 65536)
            {
                return Variable.TiposDatos.Int;
            }
            return Variable.TiposDatos.Float;
        }
        // Libreria -> #include<Identificador(.h)?>
        private void Libreria()
        {
            match("#");
            match("include");
            match("<");
            match(Tipos.Identificador);
            if (getContenido() == ".")
            {
                match(".");
                match("h");
            }
            match(">");
        }
        //Librerias -> Libreria Librerias?
        private void Librerias()
        {
            Libreria();
            if (getContenido() == "#")
            {
                Librerias();
            }
        }
        //Variables -> tipo_dato ListaIdentificadores; Variables?
        private void Variables()
        {
            Variable.TiposDatos tipo = Variable.TiposDatos.Char;
            switch (getContenido())
            {
                case "int": tipo = Variable.TiposDatos.Int; break;
                case "float": tipo = Variable.TiposDatos.Float; break;
            }
            match(Tipos.TipoDato);
            ListaIdentificadores(tipo);
            match(";");
            if (getClasificacion() == Tipos.TipoDato)
            {
                Variables();
            }
        }
        //ListaIdentificadores -> identificador (,ListaIdentificadores)?
        private void ListaIdentificadores(Variable.TiposDatos tipo)
        {
            if (!Existe(getContenido()))
            {
                lista.Add(new Variable(getContenido(), tipo));
            }
            else
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> está duplicada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == ",")
            {
                match(",");
                ListaIdentificadores(tipo);
            }
        }
        //BloqueInstrucciones -> { ListaInstrucciones ? }
        private void BloqueInstrucciones(bool ejecuta, bool primeraVez)
        {
            match("{");
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta, primeraVez);
            }
            match("}");
        }

        //ListaInstrucciones -> Instruccion ListaInstrucciones?
        private void ListaInstrucciones(bool ejecuta,bool primeraVez)
        {
            Instruccion(ejecuta,primeraVez);
            if (getContenido() != "}")
            {
                ListaInstrucciones(ejecuta,primeraVez);
            }
        }
        //Instruccion -> Printf | Scanf | If | While | Do | For | Asignacion
        private void Instruccion(bool ejecuta,bool primeraVez)
        {
            if (getContenido() == "printf")
            {
                Printf(ejecuta);
            }
            else if (getContenido() == "scanf")
            {
                Scanf(ejecuta);
            }
            else if (getContenido() == "if")
            {
                If(ejecuta,primeraVez);
            }
            else if (getContenido() == "while")
            {
                While(ejecuta,primeraVez);
            }
            else if (getContenido() == "do")
            {
                Do(ejecuta,primeraVez);
            }
            else if (getContenido() == "for")
            {
                For(ejecuta,primeraVez); 
            }
            else
            {
                Asignacion(ejecuta,primeraVez);
            }
        }
        //Asignacion -> identificador = Expresion;
        private void Asignacion(bool ejecuta,bool primeraVez)
        {
            float resultado = 0;
            tipoDatoExpresion = Variable.TiposDatos.Char;
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            log.Write(getContenido() + " = ");
            string variable = getContenido();
            match(Tipos.Identificador);
            float valor = getValor(variable);
            resultado = valor;
            if (getContenido() == "=")
            {
                match("=");
                Expresion(primeraVez);
                resultado = stack.Pop();
                asm.WriteLine("POP AX");
                asm.WriteLine("; ASIGNACION "+variable);
                asm.WriteLine("MOV "+variable+", AX");
            }
            else if (getClasificacion() == Tipos.IncrementoTermino)
            {
                if (getContenido() == "++")
                {
                    match("++");
                    if (ejecuta)
                    {
                        resultado++;
                    }
                }
                else if (getContenido() == "--")
                {
                    match("--");
                    if (ejecuta)
                    {
                        resultado--;
                    }
                }
                else if (getContenido() == "+=")
                {
                    match("+=");
                    Expresion(primeraVez);
                    resultado += stack.Pop();
                    asm.WriteLine("POP AX");
                }
                else if (getContenido() == "-=")
                {
                    match("-=");
                    Expresion(primeraVez);
                    resultado -= stack.Pop();
                    asm.WriteLine("POP AX");
                }
            }
            else if (getClasificacion() == Tipos.IncrementoFactor)
            {
                string operador = getContenido();
                match(operador);
                Expresion(primeraVez);
                if (ejecuta)
                {

                    if (operador == "*=")
                        {resultado *= stack.Pop();
                        asm.WriteLine("POP AX");}
                    else if (operador == "/=")
                        {resultado /= stack.Pop();
                        asm.WriteLine("POP AX");}
                    else if (operador == "%=")
                        {resultado %= stack.Pop();
                        asm.WriteLine("POP AX");}

                }

            }
            log.WriteLine(" = " + resultado);
            if (ejecuta)
            {
                Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                Variable.TiposDatos tipoDatoResultado = getTipo(resultado);
                /* Console.WriteLine(variable + " = "+tipoDatoVariable);
                 Console.WriteLine(resultado + " = "+tipoDatoResultado);
                 Console.WriteLine("expresion = "+tipoDatoExpresion); */
                Variable.TiposDatos tipoDatoMayor;
                if(tipoDatoExpresion>=tipoDatoResultado){
                    tipoDatoMayor=tipoDatoExpresion;
                }
                else{
                    tipoDatoMayor=tipoDatoExpresion;
                }
                if (tipoDatoVariable >= tipoDatoMayor)
                {
                    Modifica(variable, resultado);
                }
                else
                {
                    throw new Error("de semantica, no se puede asignar in <" + tipoDatoMayor + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                }
            }
            match(";");
        }
        //While -> while(Condicion) BloqueInstrucciones | Instruccion
        private void While(bool ejecuta,bool primeraVez)
        {
            int inicia = caracter;
            int lineaInicio = linea;

            do
            {
                match("while");
                match("(");
                string variable = getContenido();
                log.WriteLine("while: " + variable);
                ejecuta = Condicion("",primeraVez) && ejecuta;
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta,primeraVez);
                }
                else
                {
                    Instruccion(ejecuta,primeraVez);
                }
                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 5;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;

                }
            }
            while (ejecuta);

        }
        //Do -> do BloqueInstrucciones | Instruccion while(Condicion)
        private void Do(bool ejecuta, bool primeraVez)
        {
            int inicia = caracter;
            int lineaInicio = linea;

            do
            {
                match("do");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta,primeraVez);
                }
                else
                {
                    Instruccion(ejecuta,primeraVez);
                }
                match("while");
                match("(");
                string var = getContenido();
                log.WriteLine("Do while: " + var);
                ejecuta = Condicion("",primeraVez) && ejecuta;
                match(")");
                match(";");
                if (ejecuta)
                {
                    archivo.DiscardBufferedData();
                    caracter = inicia - 2;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;

                }
            }
            while (ejecuta);
        }
        //For -> for(Asignacion Condicion; Incremento) BloqueInstrucciones | Instruccion
        private void For(bool ejecuta, bool primeraVez)
        {
            asm.WriteLine("; For: "+contFor);
            match("for");
            match("(");
            Asignacion(ejecuta,primeraVez);

            string etiquetaInicio = "InicioFor"+ contFor;
            string etiquetaFin    = "FinFor"+ contFor++;

            int inicia = caracter;
            int lineaInicio = linea;
            float resultado = 0;
            string variable = getContenido();
             primeraVez = true;

            log.WriteLine("for: " + variable);
            asm.WriteLine(etiquetaInicio+":");
            do
            {
                ejecuta = Condicion(etiquetaFin,primeraVez) && ejecuta;
                match(";");
                resultado = Incremento(ejecuta);
                match(")");
                if (getContenido() == "{")
                {
                    BloqueInstrucciones(ejecuta,primeraVez);
                }
                else
                {
                    Instruccion(ejecuta,primeraVez);
                }
                if (getValor(variable) < resultado)
                {
                    asm.WriteLine("INC " + variable);
                }
                else if (getValor(variable) > resultado)
                {
                    asm.WriteLine("DEC " + variable);
                }
                if (ejecuta)
                {
                    Variable.TiposDatos tipoDatoVariable = getTipo(variable);
                    Variable.TiposDatos tipoDatoResultado = getTipo(resultado);
                    if (tipoDatoVariable >= tipoDatoResultado)
                    {
                        Modifica(variable, resultado);
                    }
                    else
                    {
                        throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                    }
                    archivo.DiscardBufferedData();
                    caracter = inicia - variable.Length - 1;
                    archivo.BaseStream.Seek(caracter, SeekOrigin.Begin);
                    nextToken();
                    linea = lineaInicio;
                    
                }
                asm.WriteLine("JMP " + etiquetaInicio);
                primeraVez = false;
            }
            while (ejecuta);
            asm.WriteLine(etiquetaFin+":");
        }

        //Incremento -> Identificador ++ | --
        private float Incremento(bool ejecuta)
        {
            string variable = getContenido();
            float resultado = 0;
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            match(Tipos.Identificador);
            if (getContenido() == "++")
            {
                match("++");
                if (ejecuta)
                {
                    resultado = getValor(variable) + 1;
                }
            }
            else
            {
                match("--");
                if (ejecuta)
                {
                    resultado = getValor(variable) - 1;
                }
            }
            return resultado;
        }
        //Condicion -> Expresion OperadorRelacional Expresion
        private bool Condicion(string etiqueta,bool primeraVez)
        {
            Expresion(primeraVez);
            string operador = getContenido();
            match(Tipos.OperadorRelacional);
            Expresion(primeraVez);
            float R1 = stack.Pop();//Expresion 2
            float R2 = stack.Pop();//Expresion 1
            if(primeraVez){
                asm.WriteLine("POP BX");//Expresion 2
                asm.WriteLine("POP AX");//Expresion 1
                asm.WriteLine("CMP BX,AX");
            }
            switch (operador)
            {
                case "==":
                if(primeraVez) asm.WriteLine("JNE"+etiqueta);
                return R2 == R1;
                case ">": 
                if(primeraVez) asm.WriteLine("JBE"+etiqueta);
                return R2 > R1;
                case ">=": 
                if(primeraVez) asm.WriteLine("JB"+etiqueta);
                return R2 >= R1;
                case "<": 
                if(primeraVez) asm.WriteLine("JAE"+etiqueta);
                return R2 < R1;
                case "<=": 
                if(primeraVez) asm.WriteLine("JA"+etiqueta);
                return R2 <= R1;
                default: 
                if(primeraVez) asm.WriteLine("JE"+etiqueta);
                return R2 != R1;
            }
        }
        //If -> if (Condicion) BloqueInstrucciones | Instruccion (else BloqueInstrucciones | Instruccion)?
        private void If(bool ejecuta,bool primeraVez)
        {
            match("if");
            match("(");
            asm.WriteLine("; if:"+contIf);
            string etiqueta="Eif"+contIf;
            bool evaluacion = Condicion(etiqueta,primeraVez) && ejecuta;
            match(")");
            if (getContenido() == "{")
            {
                BloqueInstrucciones(evaluacion&&ejecuta,primeraVez);
            }
            else
            {
                Instruccion(evaluacion&&ejecuta,primeraVez);
            }
            asm.WriteLine("Etiqueta1:");
            if (getContenido() == "else")
            {
                match("else");
                if (ejecuta)
                {
                    if (getContenido() == "{")
                    {
                        BloqueInstrucciones(!evaluacion&&ejecuta,primeraVez);
                    }
                    else
                    {
                        Instruccion(!evaluacion&&ejecuta,primeraVez);
                    }
                }
            }
        }
        //Printf -> printf(cadena(,Identificador)?);
        private void Printf(bool ejecuta)
        {
            match("printf");
            match("(");
            if (ejecuta)
            {
                Console.Write(getContenido().Replace("\\n", "\n").Replace("\\t", "\t").Replace("\"", ""));
            }
            match(Tipos.Cadena);
            if (getContenido() == ",")
            {
                match(",");
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                if (ejecuta)
                {
                    string Variable = getContenido();
                    Console.Write(getValor(Variable));
                }
                match(Tipos.Identificador);
            }
            match(")");
            match(";");
        }
        //Scanf -> scanf(cadena,&Identificador);
        private void Scanf(bool ejecuta)
        {
            match("scanf");
            match("(");
            match(Tipos.Cadena);
            match(",");
            match("&");
            if (!Existe(getContenido()))
            {
                throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
            }
            string variable = getContenido();
            Variable.TiposDatos tipoDatoVariable = getTipo(variable);
            match(Tipos.Identificador);
            if (ejecuta)
            {
                string captura = "" + Console.ReadLine();
                if (float.TryParse(captura, out float resultado))
                {
                    stack.Push(resultado);
                    tipoDatoVariable = getTipo(variable);
                    Variable.TiposDatos tipoDatoResultado = getTipo(resultado);
                    if (tipoDatoVariable >= tipoDatoResultado)
                    {
                       asm.WriteLine("call scan_num");
                       asm.WriteLine("MOV "+variable+", CX");
                        Modifica(variable, resultado);
                    }
                    else
                    {
                        throw new Error("de semantica, no se puede asignar in <" + tipoDatoResultado + "> a un <" + tipoDatoVariable + ">", log, linea, columna);
                    }
                }
                else
                {
                    throw new Exception("Se capturó una cadena en lugar de un número en la variable <" + variable + ">");
                }
            }
            match(")");
            match(";");
        }

        //Main -> void main() BloqueInstrucciones
        private void Main(bool ejecuta)
        {
            match("void");
            match("main");
            match("(");
            match(")");
            BloqueInstrucciones(ejecuta,true);
        }
        //Expresion -> Termino MasTermino
        private void Expresion(bool primeraVez)
        {
            Termino(primeraVez);
            MasTermino(primeraVez);
        }
        //MasTermino -> (OperadorTermino Termino)?
        private void MasTermino(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorTermino)
            {
                string operador = getContenido();
                match(Tipos.OperadorTermino);
                Termino(primeraVez);
                log.Write(" " + operador);
                float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (primeraVez){
                     asm.WriteLine("POP BX");
                     asm.WriteLine("POP AX");
                }
                if (operador == "+")
                   { 
                    stack.Push(R1 + R2);
                    if(primeraVez){
                    asm.WriteLine("ADD BX,AX");
                    asm.WriteLine("PUSH BX");}
                    }
                else
                   {
                     stack.Push(R1 - R2);
                    if(primeraVez){
                    asm.WriteLine("SUB BX,AX");
                    asm.WriteLine("PUSH BX");
                    }
                   }
            }
        }
        //Termino -> Factor PorFactor
        private void Termino(bool primeraVez)
        {
            Factor(primeraVez);
            PorFactor(primeraVez);
        }
        //PorFactor -> (OperadorFactor Factor)?
        private void PorFactor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.OperadorFactor)
            {
                string operador = getContenido();
                match(Tipos.OperadorFactor);
                Factor(primeraVez);
                log.Write(" " + operador);
                 float R2 = stack.Pop();
                float R1 = stack.Pop();
                if (primeraVez)
                {
                    asm.WriteLine("POP BX");
                    asm.WriteLine("POP AX");
                }
                if (operador == "*")
                  {
                      stack.Push(R1 * R2);
                     if(primeraVez)
                     { asm.WriteLine("MUL BX");
                      asm.WriteLine("PUSH AX");
                     }
                  }
                else if (operador == "%")  // Agrega el operador '%' aquí
                 {  stack.Push(R1 % R2);
                  if(primeraVez){
                      asm.WriteLine("DIV BX");
                    asm.WriteLine("PUSH DX");}
                 }
                else
                    {
                        stack.Push(R1 / R2);
                       if(primeraVez){
                         asm.WriteLine("DIV BX");
                        asm.WriteLine("PUSH AX");
                       }
                        }

            }
        }
        //Factor -> numero | identificador | (Expresion)
        private void Factor(bool primeraVez)
        {
            if (getClasificacion() == Tipos.Numero)
            {
                log.Write(" " + getContenido());
                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, "+getContenido());
                    asm.WriteLine("PUSH AX");
                }
                stack.Push(float.Parse(getContenido()));
                if (tipoDatoExpresion < getTipo(float.Parse(getContenido())))
                {
                    tipoDatoExpresion = getTipo(float.Parse(getContenido()));
                }
                match(Tipos.Numero);
            }
             else if (getClasificacion() == Tipos.Identificador)
            {
                if (!Existe(getContenido()))
                {
                    throw new Error("de sintaxis, la variable <" + getContenido() + "> no está declarada", log, linea, columna);
                }
                if (primeraVez)
                {
                    asm.WriteLine("MOV AX, "+getContenido());
                    asm.WriteLine("PUSH AX");
                }
                stack.Push(getValor(getContenido()));
                match(Tipos.Identificador);
                if (tipoDatoExpresion < getTipo(getContenido()))
                {
                    tipoDatoExpresion = getTipo(getContenido());
                }
            }
             else
            {
                bool huboCast = false;
                Variable.TiposDatos tipoDatoCast = Variable.TiposDatos.Char;
                match("(");
                if (getClasificacion() == Tipos.TipoDato)
                {
                    huboCast = true;
                    switch (getContenido())
                    {
                        case "int": tipoDatoCast = Variable.TiposDatos.Int; break;
                        case "float": tipoDatoCast = Variable.TiposDatos.Float; break;
                    }
                    match(Tipos.TipoDato);
                    match(")");
                    match("(");
                }
                Expresion(primeraVez);
                match(")");
                if (huboCast)
                {
                    tipoDatoExpresion = tipoDatoCast;
                    stack.Push(castea(stack.Pop(), tipoDatoCast));
                    if (primeraVez)
                    {
                        asm.WriteLine("POP AX");
                    }
                }
            }
        }
           float castea(float resultado, Variable.TiposDatos tipoDato)
        {
            switch (tipoDato)
            {
                case Variable.TiposDatos.Char: return MathF.Round(resultado) % 256;
                case Variable.TiposDatos.Int : return MathF.Round(resultado) % 65536;
            }
            return resultado;
        }
    }
}