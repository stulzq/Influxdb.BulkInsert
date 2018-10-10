# Influxdb.BulkInsert

Influxdb 批量插入组件，支持http和udp协议，目前不支持https

>http协议待完善和测试

## 使用规范

### 1.写入的数据**末尾**字符不得带换行 ”\n“

这种不被允许:
````sql
test,type='cpu' value=1\n
test,type='cpu' value=1\ntest,type='cpu' value=1\n
````

这种允许：
````sql
test,type='cpu' value=1
test,type='cpu' value=1\ntest,type='cpu' value=1
````

### 2.写入数据必须带上时间戳

因为发送数据是按批次发送，如果批次的数据数量大于1（BitchSize）,那么数据是合并发送的，这时需要每条数据都带上时间戳，不然数据将会被覆盖。可以使用 `InfluxdbTimeGen.GenNanosecond()`来生成纳秒时间戳，时间戳必须为纳秒。

### 3.数据格式

````sql
<measurement>,<tags> <fields> <time>
````

示例：

````sql
test,type='cpu' value=1 1539076079817999232
test,type='cpu' value=1 1539076079817999232\ntest,type='cpu' value=1 1539076079817999232
````

