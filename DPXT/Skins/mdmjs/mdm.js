//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size

if (!Array.prototype.find) {//让IE支持下array find方法
    Array.prototype.find = function (predicate) {
        if (this === null) {
            throw new TypeError('Array.prototype.find called on null or undefined');
        }
        if (typeof predicate !== 'function') {
            throw new TypeError('predicate must be a function');
        }
        var list = Object(this);
        var length = list.length >>> 0;
        var thisArg = arguments[1];
        var value;

        for (var i = 0; i < length; i++) {
            value = list[i];
            if (predicate.call(thisArg, value, i, list)) {
                return value;
            }
        }
        return undefined;
    };
}

//让IE支持字串对象的startsWidth方法
if (!String.prototype.startsWith) {
    String.prototype.startsWith = function (searchString, position) {
        position = position || 0;
        return this.substr(position, searchString.length) === searchString;
    };
}

//让IE支持字串对象的endsWith方法
if (!String.prototype.endsWith) {
    String.prototype.endsWith = function (searchString, position) {
        var subjectString = this.toString();
        if (typeof position !== 'number' || !isFinite(position) || Math.floor(position) !== position || position > subjectString.length) {
            position = subjectString.length;
        }
        position -= searchString.length;
        var lastIndex = subjectString.indexOf(searchString, position);
        return lastIndex !== -1 && lastIndex === position;
    };
}

function autoPanelHeight() {
    var target = $('.panel-auto-height .auto-height-container');
    if (target.length == 0) return;
    target.each(function () {
        var winHeight = $(window).height(),
            targetTop = $(this).offset().top,
            bottomHeight = 120,
            targetWrapper = $(this).closest('.panel-auto-height'),
            targetWrapperTop = targetWrapper.offset().top;
        // if(target.find('li').length==0){
        //     bottomHeight=178;
        // }
        $(this).css({
            'height': winHeight - targetTop - bottomHeight - 15 + 'px',
            'overflow-y': 'auto'
        }).closest('.panel-auto-height').css({
            'height': winHeight - targetWrapperTop - bottomHeight + 'px'
        })

    })

}

$(function () {
    $(window).bind("load resize", function () {
        var topOffset = 71;
        var width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        var height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });
});
//选中某个指定菜单。menuState: mdm.application_index
function selectMenu(menuState) {
    menuState = menuState.replace(/\./g, '/');
    $("#side-menu .active,#side-menu .in").each(function () {
        $(this).removeClass('active');
        $(this).removeClass('in');
    });
    var element = $('#side-menu ul.nav a').filter(function () {
        var url = this.href;
        return url.indexOf(menuState) >= 0;
    });
    if (element.length == 0) {
        window.__selectMenuTargetUri = menuState;
        return;
    }


    element.parent().addClass('active').closest('ul').addClass('in').parent().addClass('active');
}
