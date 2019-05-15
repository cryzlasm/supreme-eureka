private static void LoadDLLs()
{
    AppDomain.CurrentDomain.AssemblyResolve += delegate(object sender, ResolveEventArgs bargs)
    {
        string text = bargs.Name;
        if (bargs.Name.Contains(","))
        {
            text = text.Split(new char[]
            {
                ','
            })[0];
        }
        string value = new AssemblyName(text).Name + ".dll";
        Assembly executingAssembly = Assembly.GetExecutingAssembly();
        string[] manifestResourceNames = executingAssembly.GetManifestResourceNames();
        string text2 = null;
        foreach (string text3 in manifestResourceNames)
        {
            if (text3.Contains(value))
            {
                text2 = text3;
            }
        }
        if (text2 == null)
        {
            return null;
        }
        Assembly result;
        using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(text2))
        {
            byte[] array2 = new byte[manifestResourceStream.Length];
            manifestResourceStream.Read(array2, 0, array2.Length);
            result = Assembly.Load(array2);
        }
        return result;
    };
}