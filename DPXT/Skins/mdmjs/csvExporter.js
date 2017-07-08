function csvExporter() {
    var isIE = function (ver) {
        var b = document.createElement('b')
        b.innerHTML = '<!--[if IE ' + ver + ']><i></i><![endif]-->'
        return b.getElementsByTagName('i').length === 1
    }

    function exportTable(tableId, filename) {
        var data = getCsvDataByTableId(tableId);
        download(filename, data);
    };

    function exportJson(jsonObject, showHeader, filename) {
        var data = getCsvDataByJsonObject(jsonObject, showHeader);
        download(filename, data);
    };

    function download(fileName, data) {
        fileName = fileName + ".csv";

        if (isIE(9)) {
            var IEwindow = window.open();
            IEwindow.document.write('sep=,\r\n' + data);
            IEwindow.document.close();
            IEwindow.document.execCommand('SaveAs', true, fileName);
            IEwindow.close();
        }
        else {
            var charset = document.characterSet;
            var uri = 'csv/txt;charset=' + charset;

            saveAs(new Blob([data], {
                type: uri
            }), fileName);
        }
    }

    function fixCSVField(value) {
        var fixedValue = value + '';
        var addQuotes = (value.indexOf(',') !== -1) || (value.indexOf('\r') !== -1) || (value.indexOf('\n') !== -1);
        var replaceDoubleQuotes = (value.indexOf('"') !== -1);

        if (replaceDoubleQuotes) {
            fixedValue = fixedValue.replace(/"/g, '""');
        }
        if (addQuotes || replaceDoubleQuotes) {
            fixedValue = '"' + fixedValue + '"';
        }
        return fixedValue;
    };


    function getCsvDataByTableId(tableId) {
        var table = document.getElementById(tableId);
        if (!table) {
            alert(tableId + " 不存在！");
            return;
        }

        var data = '\ufeff';
        if (table.rows.length == 0)
            return data;

        var actionColIndex = -1;
        for (var i = 0, row; row = table.rows[i]; i++) {
            for (var j = 0, col; col = row.cells[j]; j++) {
                var text = getText(col);
                if (text == '操作') {
                    //是否存在操作列，如果存在就不导出这一列
                    actionColIndex = j;
                }

                if (j == actionColIndex) {
                    continue;
                }
                data = data + (j ? ',' : '') + fixCSVField(text);
            }
            data = data + '\r\n';
        }
        return data;
    }

    function getText(el) {
        var s = el.textContent || el.innerText;
        return s == null ? "" : s.replace(/^\s*(.*?)\s+$/, "$1");
    }

    function getCsvDataByJsonObject(array, showHeader) {
        var data = '\ufeff';
        if (!array || !Array.isArray(array) || array.length == 0) {
            throw "非数组不可导出！";
        }

        if (showHeader) {
            var row = "";
            for (var index in array[0]) {
                row += index + ',';
            }
            row = row.slice(0, -1);
            data += row + '\r\n';
        }

        for (var i = 0; i < array.length; i++) {

            var row = "";
            if (typeof array[i] != 'object') {
                //如果不是对象，就是简单类型，那么直接输出内容即可
                row += fixCSVField(array[i] + '');
            }
            else if (Array.isArray(array[i])) {
                //数组的话，必须是index从0开始遍历。
                for (var index = 0; index < array[i].length; index++) {
                    var text = getObjectText(array[i][index]);
                    row += (index ? ',' : '') + fixCSVField(text);
                }
            } else {
                //对象的话，就遍历对象的属性
                for (var key in array[i]) {
                    var text = getObjectText(array[i][key]);
                    row += fixCSVField(text) + ',';
                }
                row = row.slice(0, -1);
            }
            data += row + '\r\n';
        }

        return data;
    }

    function getObjectText(el) {

        if (typeof el === 'undefined' || el == null) {
            return '';
        }

        var s = '' + el;
        return s.replace(/^\s*(.*?)\s+$/, "$1");
    }

    return {exportTable: exportTable, exportJson: exportJson};
}

