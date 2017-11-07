using System;
using System.Collections.Generic;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Reflection;
using System.IO;

namespace JsonSharp
{
    internal class CSharpSourceCompiler
    {
        public static string FullReference(string relativeReference)
        {
            // First, get the path for this executing assembly.
            Assembly a = Assembly.GetExecutingAssembly();
            string path = Path.GetDirectoryName(a.Location);

            // if the file exists in this Path - prepend the path
            string fullReference = Path.Combine(path, relativeReference);
            if (File.Exists(fullReference))
                return fullReference;
            else
            {
                // Strip off any trailing ".dll" if present.
                if (string.Compare(relativeReference.Substring(relativeReference.Length - 4), ".dll", true) == 0)
                    fullReference = relativeReference.Substring(0, relativeReference.Length - 4);
                else
                    fullReference = relativeReference;

                // See if the required assembly is already present in our current AppDomain
                foreach (Assembly currAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (string.Compare(currAssembly.GetName().Name, fullReference,
                    true) == 0)
                    {
                        // Found it, return the location as the full reference.
                        return currAssembly.Location;
                    }
                }

                // The assembly isn't present in our current application, so attempt to
                // load it from the GAC, using the partial name.
                try
                {
                    Assembly tempAssembly = Assembly.Load(fullReference);
                    return tempAssembly.Location;
                }
                catch
                {
                    // If we cannot load or otherwise access the assembly from the GAC then just
                    // return the relative reference and hope for the best.
                    return relativeReference;
                }
            }
        }

        public static Type[] CompileAndReturnInstance(string source)
        {
            CodeDomProvider provider = 
                CodeDomProvider.CreateProvider("CSharp", new Dictionary<string, string>() { { "CompilerVersion", "v4.0" } });

            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;
            cp.ReferencedAssemblies.Add(FullReference("MessagePack.dll"));

            //string libDir = AppDomain.CurrentDomain.BaseDirectory;
            //libDir.TrimEnd('\\');
            //cp.CompilerOptions = string.Format("/lib:\"{0}\"", libDir);
            CompilerResults cr = provider.CompileAssemblyFromSource(cp, source);

            foreach (CompilerError ce in cr.Errors)
            {
                if (ce.IsWarning) continue;
                Console.WriteLine("{0}({1},{2}: error {3}: {4}", ce.FileName, ce.Line, ce.Column, ce.ErrorNumber, ce.ErrorText);
            }

            Type[] types = cr.CompiledAssembly.GetTypes();
            foreach (Type type in types)
            {
                Console.WriteLine("{0}, {1}", type.Name, type.Assembly.FullName);
            }

            return types;
        }
    }
}
