using System.Collections.Generic;

public class BaiduFaceData
{
    public int error_code { get; set; }
    public string error_msg { get; set; }
    public long log_id { get; set; }
    public long timestamp { get; set; }
    public int cached { get; set; }
    public Result result { get; set; }
}

public class Result
{
    public string face_token { get; set; }
    public List<User> user_list { get; set; }
}

public class User
{
    public string group_id { get; set; }
    public string user_id { get; set; }
    public string user_info { get; set; }
    public double score { get; set; }
}
