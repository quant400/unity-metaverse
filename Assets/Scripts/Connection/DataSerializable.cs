using System;
using System.Collections.Generic;

[Serializable]
public class Attribute
{
    public string trait_type;
    public string value;
}

[Serializable]
public class AllUrl
{
    public string name;
    public string link;
}

[Serializable]
public class Account
{
    public string name;
    public int id;
    public string image;
    public string external_url;
    public string description;
    public List<Attribute> attributes;
    public string animation_url;
    public List<AllUrl> all_urls;
    public bool isNFTForTesting;
}

[Serializable]
public class RootAccount
{
    public List<Account> accounts;
}