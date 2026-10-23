using System;

[Serializable]
public class Contact
{
    public int id;
    public string first_name;
    public string last_name;
    public string email;
    public string gender;
    public string ip_address;

    public string FullName => $"{first_name} {last_name}".Trim();
}

[Serializable]
public class ContactData
{
    public Contact[] data;
}
