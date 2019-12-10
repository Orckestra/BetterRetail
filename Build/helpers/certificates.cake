using System.Security.Cryptography.X509Certificates;

public static X509Certificate2 GetCertificate(string name)
{
    using (var localStore = new X509Store(StoreLocation.LocalMachine))
    using (var userStore = new X509Store(StoreLocation.CurrentUser))
    {
        localStore.Open(OpenFlags.ReadOnly);
        userStore.Open(OpenFlags.ReadOnly);

        var certificate = localStore.Certificates.OfType<X509Certificate2>()
            .Union(userStore.Certificates.OfType<X509Certificate2>())
            .Where(x => x.Subject.Contains(name))
            .FirstOrDefault();

        if (certificate == null)
            throw new Exception($"Certificate '{name}' not found. It must be installed");

        localStore.Close();
        userStore.Close();

        return certificate;
    }
}
