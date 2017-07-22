//获得tr的json集合
function GetTrJson(tableid, startrowindex) {
    var HtmlTableJson = "";     //表格对应Json字符串
    var RowIndex = startrowindex;           //行索引
    $("#" + tableid).find("tr").each(function () {
        var Json_Tr = "";
        Json_Tr += "{";
        Json_Tr += "\"rowindex\":" + RowIndex + ","; //行索引
        Json_Tr += "\"cells\":[";

        var CellIndex = 0;   //单元格索引
        //表头th元素
        $(this).find("th").each(function () {
            var Json_Td = "";
            Json_Td += "{";

            Json_Td += "\"cellindex\":" + CellIndex + ",";
            Json_Td += "\"content\":\"" + $(this).text() + "\",";
            Json_Td += "\"colspan\":" + ($(this).attr("colspan") ? $(this).attr("colspan") : "1") + ",";
            Json_Td += "\"rowspan\":" + ($(this).attr("rowspan") ? $(this).attr("rowspan") : "1");

            Json_Td += "},";
            CellIndex += 1; //更新单元格索引
            Json_Tr += Json_Td;//将单元格对象添加至行对象中
        });

        ////内容td元素
        $(this).find("td").each(function () {
            var Json_Td = "";
            Json_Td += "{";

            Json_Td += "\"cellindex\":" + CellIndex + ",";
            Json_Td += "\"content\":\"" + $(this).text() + "\",";
            Json_Td += "\"colspan\":" + ($(this).attr("colspan") ? $(this).attr("colspan") : "1") + ",";
            Json_Td += "\"rowspan\":" + ($(this).attr("rowspan") ? $(this).attr("rowspan") : "1");

            Json_Td += "},";
            CellIndex += 1; //更新单元格索引
            Json_Tr += Json_Td;//将单元格对象添加至行对象中
        });

        //截取最后一个符号","
        if (Json_Tr.length > 0) {
            Json_Tr = Json_Tr.substr(0, Json_Tr.length - 1);
        }

        Json_Tr += "]},";
        RowIndex += 1;//更新行索引
        HtmlTableJson += Json_Tr;//将行对象添加至结合中
    });
    if (HtmlTableJson.length > 0) {
        HtmlTableJson = HtmlTableJson.substr(0, HtmlTableJson.length - 1);
    }
    return HtmlTableJson;
}