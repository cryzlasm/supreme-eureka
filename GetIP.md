# <center>  用于获得外网IP 搜集的一些接口 </center>

> **总体来说现在国内的情况, 外网IP资源都很紧张, 基本获取的外网IP很大概率上只是小区网的出口**


```bash
验证IP代理是否高匿: http://httpbin.org/ip


获取外网IP
checkip.dyndns.com

JSon
curl ipinfo.io/json
curl ifconfig.me/all.json
curl www.trackip.net/ip?json  // 比较详细
curl api.ipify.org?format=json  // 简单方便


http://icanhazip.com/
http://checkip.amazonaws.com/
http://www.net.cn/static/customercare/yourip.asp
http://ip.qq.com/cgi-bin/myip
http://ad.airll.com/system/ipget.asp?md=898&getip=
http://ip.chinaz.com/getip.aspx     // JS代码
http://api.ipify.org?format=json    //JSON
http://www.ipchicken.com




curl api.ipify.org?format=json
curl checkip.amazonaws.com
curl icanhazip.com
curl ipinfo.io/ip
curl ipecho.net/plain
curl www.trackip.net/i
curl whatismyip.akamai.com
curl inet-ip.info
curl ip.cn     


// 查询IP信息
curl http://api.db-ip.com/v2/free/118.5.49.6
{
    "ipAddress": "118.5.49.6",
    "continentCode": "AS",
    "continentName": "Asia",
    "countryCode": "JP",
    "countryName": "Japan",
    "stateProv": "Hiroshima",
    "city": "Minami Ward (Niho)"
}


```
