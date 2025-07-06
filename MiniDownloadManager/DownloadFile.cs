using System.Collections.Generic;

public class DownloadFile
{
    public string Title { get; set; }
    public string ImageURL { get; set; }
    public string FileURL { get; set; }
    public int Score { get; set; }
    public Dictionary<string, int> Validators { get; set; }
}
