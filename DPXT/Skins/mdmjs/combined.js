'use strict';
/**
 * @ngdoc overview
 * @name mdmApp
 * @description
 * # mdmApp
 *
 * Main module of the application.
 */
var mdmApp = angular
    .module('mdmApp', [
        'oc.lazyLoad',
        'ui.router',
        'ui.bootstrap',
        'angular-loading-bar',
        'ngMessages',
        'angularUtils.directives.dirPagination',
        'angular-sortable-view',
        'ngzTree',
        'angularShiro',
        'ngCookies'

    ]);

angular.controllers = [];
//decorator to store all controllers defined by us
(function (orig) {
    mdmApp.controller = function () {
        if (arguments.length > 1) {
            var parts = arguments[0].split("?");
            var realControllerName = parts[0];
            var queryParams = parts[1] || "";

            arguments[0] = realControllerName;
            angular.controllers.push({name: realControllerName, params: queryParams});
        }
        return orig.apply(null, arguments);
    }
})(mdmApp.controller);

mdmApp
    .constant("appConstants", {
        //ajax服务器端地址
        "domain": "http://localhost:8080",//服务端API域名
        "loginUrl": "/account/authenticate",
        //消息的级别定义
        messageBoxIcons: {
            "WARNING": "warning",
            "INFO": "info",
            "CRITICAL": "critical",
            "QUESTION": "question"
        },
        //默认列表页的PageSize
        pageSize: 15
    })
    .factory('httpInterceptor', function ($q, $injector, $log, $rootScope, appConstants, $cookies, $location) {
        var showMsg = function (msg) {
            $injector.get("$rootScope").$alert(appConstants.messageBoxIcons.CRITICAL, msg);
        }

        var redirectToLogin = function () {
            $cookies.remove("sid");
            $cookies.remove("rememberMe");
            $location.path("/login");
        }

        var interceptor = {
            request: function (config) {
                config.timeout = config.timeout || 60000; //默认60秒超时
                var isBusinessRequest = config.url.indexOf('$') == 0;//$开始的url请求表示是请求mdm console站点的
                if (isBusinessRequest) {
                    config.isBusinessRequest = isBusinessRequest;
                    //config.throwReject = config.throwReject;//是否向外抛出Rejeted状态
                    config.url = appConstants.domain + config.url.substring(1);
                    //统一修正下url传参的key都为小写
                    if (config.method == "GET" && config.params) {
                        //因为涉及到引用问题，在修改之前，先clone一份。
                        config.params = angular.copy(config.params);
                        for (var i in config.params) {
                            if (i.toLowerCase() != i) {
                                config.params[i.toLowerCase()] = config.params[i];
                                delete config.params[i];
                            }
                        }
                    }
                }
                //TODO:携带Authorization头
                //config.headers.comm | post | put
                config.withCredentials = true //TODO:如果需要身份传递的话
                return config;
            },
            responseError: function (res) {
                if (res.status == 403) {
                    redirectToLogin();
                    return $q.reject(res);
                }
                if (res && res.data && res.data.message) {
                    if (res.config.isBusinessRequest) {
                        showMsg(res.data.message);
                    }
                }
                else {
                    switch (res.status) {
                        case -1:
                            //FIXME:navigator.onLine == false
                            showMsg("很抱歉，您无法与服务器建立连接，请检查网络。");
                            break;
                        case 401:
                            showMsg("您没有授权访问这个资源。");
                            break;
                        case 404:
                            showMsg("您要访问的资源不存在。");
                            break;
                        case 500:
                            showMsg("服务发生内部错误，请您联系技术人员。");
                            break;
                        case 400:
                        default:
                            break;
                    }
                }

                return $q.reject(res);
            }
        }
        return interceptor;
    })
    .config(['$stateProvider', '$urlRouterProvider', '$httpProvider', 'angularShiroConfigProvider', 'appConstants',
        function ($stateProvider, $urlRouterProvider, $httpProvider, config, appConstants) {
            config.setAuthenticateUrl(appConstants.domain + appConstants.loginUrl);
            $httpProvider.interceptors.push('httpInterceptor');

            window.$stateProvider = $stateProvider;

            $urlRouterProvider.otherwise(function ($injector, $location) {
                return '/mdm/home';
            });

            $stateProvider
                .state('mdm', {
                    url: '/mdm',
                    templateUrl: 'views/main_template.html',
                    controller: 'mdm'
                }).state('login', {
                url: '/login',
                templateUrl: 'login.html',
                controller: 'mdm.login_index'
            });
        }]);

mdmApp.run(function ($rootScope, $injector, $location, $uibModal, $timeout, $http, $state, $cookies, appConstants, subject, authenticationResponseParser, accountService) {

    $rootScope.$on('$locationChangeStart', function (event, next, current) {
        var loginPage = $location.path() === '/login';
        var sessionId = $cookies.get('sid');
        //页面刷新时重新恢复登录状态
        if (sessionId && !subject.authenticated) {
            accountService.getAccountInfo().then(function (response) {
                var info = response.data;
                if (info) {
                    var infos = authenticationResponseParser.parse(info);
                    subject.authenticationInfo = infos.authc;
                    subject.authorizer.setAuthorizationInfo(infos.authz);
                    subject.authenticated = true;
                    $rootScope.username = subject.getPrincipal().login;
                }
            }).then(function () {
                if (loginPage) {
                    $location.path('/');
                }
            });
        } else {
            if (!loginPage && !sessionId) {
                $location.path('/login');
            } else if (loginPage && sessionId) {
                    $location.path('/');
                }
            }

        });

    $rootScope.$on('$viewContentLoaded', function () {
        $(window).resize();
    });

    $rootScope.$goByUiSref = function (uiSref) {
        var bracketIndex = uiSref.indexOf("(");
        if (bracketIndex < 0) {
            $state.go(uiSref);
            return;
        }
        var lastBracketIndex = uiSref.lastIndexOf(")");

        var stateName = uiSref.substring(0, bracketIndex);
        var paramsInText = uiSref.substring(bracketIndex + 1, lastBracketIndex);

        var params = JSON.parse(paramsInText);
        $state.go(stateName, params);
    }

    $rootScope.$confirm = function (title, content, onlyOkBtn) {
        function confirmModal($scope, appConstants) {

                $scope.content = content || "Your content here!";

                $scope.title = title || "Your title";

                $scope.icon = appConstants.messageBoxIcons.QUESTION;

            $scope.onlyOkBtn = onlyOkBtn;
            }

            var modalOptions = {
                templateUrl: 'views/common/confirm_modal.html',
                backdrop: 'static',
                controller: confirmModal
            };

            return $uibModal.open(modalOptions).result;
        };

        $rootScope.alerts = [];
        $rootScope.$alert = function (type, msg, timeout) {
            var timeout = timeout || 3000;
            var alert = {
                type: type,
                msg: msg,
                close: function (index) {
                    return closeAlert(index);
                },
                icon: function () {
                    if (this.type == 'warning') {
                        return 'fa fa-exclamation-circle';
                    } else if (this.type == 'error') {
                        return 'fa fa-times-circle';
                    } else if (this.type == 'success') {
                        return 'fa fa-check-circle';
                    }
                }
            };
            $rootScope.alerts.push(alert);

            $timeout(function () {
                var index = $rootScope.alerts.indexOf(alert);
                if (index >= 0)
                    closeAlert(index);
            }, timeout);

            function closeAlert(index) {
                $rootScope.alerts.splice(index, 1);
            };
        };
        $rootScope.$alertInfo = function (msg) {
            $rootScope.$alert(appConstants.messageBoxIcons.INFO, msg);
        }

        $rootScope.$alertWarning = function (msg) {
            $rootScope.$alert(appConstants.messageBoxIcons.WARNING, msg);
        }

        $rootScope.$alertError = function (msg) {
            $rootScope.$alert(appConstants.messageBoxIcons.CRITICAL, msg);
        }

    //region auto-gen state object to stateProvider
        if (!angular.stateCreated) {
            angular.forEach(angular.controllers, function (value, key) {
                createStateAuto(value);
            });
            window.$stateProvider = null;
            angular.stateCreated = true;
        }

        function createStateAuto(controllerDefinition) {

            //controller 名字是以dot分割的
            var dotPath = controllerDefinition.name;

            //controller上定义的查询参数
            var params = controllerDefinition.params;

            var parts = dotPath.split('.').filter(function (ele) {
                return ele.trim().length > 0;
            });

            if (parts.length == 0)
                return;

            var controllerName = '';
            var url = '';

            //如果父state没创建就挨个创建一遍
            angular.forEach(parts, function (value, key) {
                if (controllerName.length > 0) {
                    controllerName += '.';
                    url += '/';
                }
                controllerName += value;
                url += value;

                var state = $injector.get("$state").get(controllerName);
                if (state != null) {
                    //state has already there!
                    return;
                }

                //如果controller名字有单独的定义，此处不要代为创建，因为有可能controller还有其他参数
                var ctrlDef = angular.controllers.find(function (c) {
                    return c.name == controllerName;
                });
                if (ctrlDef && ctrlDef != controllerDefinition) {
                    return;
                }

                //只有到最后一级的时候，params才应该追加
                var shouldAppendQueryParam = (params.length > 0 && controllerName == dotPath);
                state = {
                    templateUrl: 'views/' + url + '.html',
                    url: "/" + value + (shouldAppendQueryParam ? "?" + params : ""),
                    controller: controllerName
                };
                window.$stateProvider.state(controllerName, state);
            });
        }

    //endregion
    }
);
mdmApp.controller("mdm", ['$scope', 'subject', 'accountService', '$cookies', '$location', function ($scope, subject, accountService, $cookies, $location) {
    $scope.logout = function () {
        $scope.$confirm('注销', '确定要退出学科网主数据管理系统吗？').then(function () {
            accountService.logout().then(function (response) {
                if (response.data) {
                    $cookies.remove("sid");
                    $cookies.remove("rememberMe");
                    $location.path("/login");
                }
            })
        })

    }
}]).filter("gradeDivisionName", function () {
    var gradeDivision = {"G54": "五四制", "G63": ""};
    return function (gradeDivisionKey) {
        return gradeDivision[gradeDivisionKey];
    }
});

/**
 * Created by Administrator on 2016/5/19.
 */
mdmApp.controller("mdm.login_index", function ($rootScope, $scope, subject, usernamePasswordToken, $cookies, $state, accountService) {
    $scope.token = usernamePasswordToken;
    $scope.token.rememberMe = true;
    $scope.login = function () {
        subject.login(angular.copy($scope.token)).then(function (data) {
            if (subject.authorizer.authorizationInfo.permissions.length == 0 && subject.authorizer.authorizationInfo.roles.length == 0) {
                accountService.logout();
                $rootScope.$alertError("用户有效，但未被授予任何权限，请联系您的管理员！");
                return;
            }

            var sessionid = subject.authenticationInfo.credentials.token;
            $rootScope.username = subject.getPrincipal().login;
            if (subject.remembered) {
                var expireDate = new Date();
                expireDate.setDate(expireDate.getDate() + 7);
                $cookies.put('sid', sessionid, {'expires': expireDate});
            } else
                $cookies.put('sid', sessionid);
            $state.go("mdm.home");
        }, function (data) {
            $rootScope.$alertError(data.message);
        });
    }
});
mdmApp.service("accountService", ['$http', function ($http) {
    var services = {};
    services.getAccountInfo = function () {
        return $http.get("$/account/userinfo");
    }
    services.logout = function () {
        return $http.get("$/account/logout");
    }
    return services;
}]);

/**
 * Created by TeckyLee on 2016/5/10.
 */
mdmApp.controller("mdm.application_index", function ($log, $uibModal, $scope, applicationService) {
    applicationService.getApplications().then(function (response) {
        $scope.appList = response.data;
    });

    $scope.editMode = 'add';
    $scope.editApp = {};
    var modalOptions = {
        templateUrl: 'views/mdm/application/edit.html',
        controller: 'mdm.application_index.edit',
        scope: $scope
    };

    //打开新增模态框
    $scope.openAddWindow = function () {
        $scope.editApp = {};
        $scope.editMode = 'add';
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            $log.info(result);
            $scope.appList.push(result);
        });
    };

    //打开更新窗口
    $scope.openEditWindow = function (app) {
        $scope.editMode = 'edit';
        $scope.editApp = angular.copy(app);
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            angular.copy(result, app);
        });
    };

    $scope.trim = function (app) {
        app.id = app.id.trim();
        app.name = app.name.trim();
        app.url && (app.url = app.url.trim());
    };

    //删除一个应用
    $scope.del = function (app) {
        var index = $scope.appList.indexOf(app);
        $scope.$confirm('删除应用', '您确定要删除应用[' + app.name + ']吗？').then(function (modalResult) {
            if (!modalResult) return;

            //执行删除
            applicationService.deleteApplication(app.id).then(function (response) {
                if (response.data) {
                    $scope.$alertInfo("删除成功！");
                    //从当前列表中删除指定项
                    $scope.appList.splice(index, 1);
                }
                else {
                    $scope.$alertError("删除失败！");
                }
            });
        });
    };

    $scope.translateStatus2Chinese = function (status) {
        if (!status || status == "") return "";
        if (status == "DISABLED") return "禁用";
        return "正常";
    };

}).controller("mdm.application_index.edit", function ($scope, applicationService) {
        $scope.submitForm = function () {
            if (!$scope.appEditForm.$valid) return;

            //trim string fields
            $scope.trim($scope.editApp);

            //提交时，根据编辑模式选择调用不同的service方法
            if ($scope.editMode == 'add') {
                applicationService.addApplication($scope.editApp).then(saveOnSuccess);
            }
            else {
                applicationService.updateApplication($scope.editApp).then(saveOnSuccess);
            }
        }

        function saveOnSuccess(response) {
            $scope.$alertInfo("保存成功！");

            //关闭窗口，并设置modal的result
            $scope.$close($scope.editApp);
        }
    })
    .service("applicationService", function ($http) {
        var services = {};
        var resUrl = '$/applications';

        services.getApplications = function () {
            return $http.get(resUrl);
        }

        services.addApplication = function (app) {
            return $http.post(resUrl, app);
        }

        services.updateApplication = function (app) {
            return $http.put(resUrl, app);
        }

        services.deleteApplication = function (id) {
            return $http.delete(resUrl + "/" + id);
        }
        return services;
    });

/**
 * Created by TeckyLee on 2016/5/11.
 */
mdmApp.controller("mdm.area_index", function ($scope, areaService) {

    $scope.ztreeId = "mdm_area_index";
    $scope.areaNodes = [];
    areaService.getAreas().then(function (response) {

        var areaList = response.data;

        for (var idx in areaList) {
            var area = areaList[idx];
            area.showName = area.shortName + " " + area.id;
        }
        $scope.areaNodes = areaList;
    });
    $scope.setting = {
        data: {
            key: {
                title: "name",
                name: "showName"
            },
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId"
            }
        },
        view: {
            showTitle: true,
            fontCss: getFontCss
        }
    }

    function getFontCss(treeId, treeNode) {
        return (!!treeNode.highlight) ? {color: "#A60000", "font-weight": "bold"} : {
            color: "#333",
            "font-weight": "normal"
        };
    }

    $scope.nodeList = [];
    $scope._query = '';
    $scope.search = function (keyEvent) {
        if (keyEvent && keyEvent.which != 13) {
            return;
        }
        $scope.query = $scope._query.trim();
        var hasValidQuery = ($scope.query && $scope.query.length > 0);
        var treeObj = $.fn.zTree.getZTreeObj($scope.ztreeId);
        updateNodes(treeObj, false);
        treeObj.hideNodes(treeObj.getNodes());

        if (hasValidQuery) {
            $scope.nodeList = treeObj.getNodesByParamFuzzy("showName", $scope.query, null);

            //显示匹配的节点
            var shownNodesList = [];
            for (var i = 0; i < $scope.nodeList.length; i++) {
                var node = $scope.nodeList[i];
                shownNodesList = shownNodesList.concat(node.getPath());
            }

            for (var i = 0; i < shownNodesList.length; i++) {
                treeObj.expandNode(shownNodesList[i], true, false);
            }

            treeObj.showNodes(shownNodesList);
            updateNodes(treeObj, true);

            //把多显示的节点去掉
            var realShownNodes = treeObj.getNodesByParam("isHidden", false);
            for (var i = 0; i < realShownNodes.length; i++) {
                var node = realShownNodes[i];
                if (shownNodesList.indexOf(node) < 0) {
                    treeObj.hideNode(node);
                }
            }
        }
        else {
            //显示所有隐藏的节点
            var allHiddenNodes = treeObj.getNodesByParam("isHidden", true);
            treeObj.showNodes(allHiddenNodes);
            treeObj.expandAll(false);
        }
    }

    function updateNodes(zTree, highlight) {

        var len = $scope.nodeList.length;
        for (var i = 0; i < len; i++) {
            $scope.nodeList[i].highlight = highlight;
            zTree.updateNode($scope.nodeList[i]);
        }

    }

    $scope.export = function () {
        csvExporter().exportJson($scope.areaNodes, true, '地区');
    }
}).service("areaService", function ($http) {
    var services = {};
    var resUrl = '$/areas';

    services.getAreas = function () {
        return $http.get(resUrl);
    }
    return services;
});


mdmApp.controller("mdm.cache_index", function ($scope, cacheService) {
    $scope.params = {key: "", pageSize: 15};
    cacheService.getCacheKeys().then(function (response) {
        $scope.cacheKeys = response.data.map(function (key) {
            var ele = {};
            ele.value = key;
            ele.selected = false;
            return ele;
        });
    });
    $scope.toggleAll = function () {
        var toggleStatus = $scope.isAllSelected;
        angular.forEach($scope.filteredCacheKeys, function (item) {
            item.selected = toggleStatus;
        });
    }

    $scope.checkToggled = function () {
        $scope.isAllSelected = $scope.filteredCacheKeys.every(function (item) {
            return item.selected;
        })
    }

    $scope.filterCache = function (cacheKey) {
        var key = $scope.params.key.trim();
        var flag = true;
        if (key) {
            flag = cacheKey.value.indexOf(key) > -1;
        }
        return flag;
    };
    $scope.delSelected = function () {
        var selectedKeys = [];
        $scope.filteredCacheKeys.forEach(function (key) {
            if (key.selected) {
                selectedKeys.push(key.value);
            }
        })
        if (selectedKeys.length == 0) {
            $scope.$alertInfo("请选择要删除的行！");
            return;
        }
        var deleteKeys = selectedKeys.join(",");
        $scope.$confirm('删除缓存', '您确定要删除选中的[' + selectedKeys.length + ']项缓存吗？').then(function (modalResult) {
            //执行删除
            cacheService.deleteByKey(deleteKeys).then(function (response) {
                $scope.$alertInfo("删除成功！");
                //从当前列表中删除指定项
                $scope.cacheKeys = $scope.cacheKeys.filter(function (key) {
                    return !key.selected;
                })
            });
        });
    }
    $scope.del = function (cacheKey) {
        var index = $scope.cacheKeys.indexOf(cacheKey);
        $scope.$confirm('删除缓存', '您确定要删除缓存[' + cacheKey.value + ']吗？').then(function (modalResult) {
            //执行删除
            cacheService.deleteByKey(cacheKey.value).then(function (response) {
                $scope.$alertInfo("删除成功！");
                //从当前列表中删除指定项
                $scope.cacheKeys.splice(index, 1);
            });
        });
    }
}).service("cacheService", function ($http) {
    var services = {};
    var url = "$/caches";
    services.getCacheKeys = function () {
        return $http.get(url);
    }

    services.deleteByKey = function (key) {
        return $http.delete(url, {"params": {"key": key}})
    }
    return services;
});

//tbid = textbook id
//tbname = textbook name
mdmApp.controller('mdm.chapter_index?tbId&tbName&returnUrl', function ($scope, $state, $stateParams, textbookService, chapterService, $timeout) {
    $scope.tbId = $stateParams.tbId;
    $scope.tbName = $stateParams.tbName;
    $scope.ztreeId = "mdm_chapter_index";
    $scope.nodes = [];
    $scope.returnUrl = ($stateParams.returnUrl ? $stateParams.returnUrl : "mdm.textbook_index");

    var newNodeIndex = 1;
    textbookService.getBookById($scope.tbId).then(function (response) {
        var book = response.data;
        $scope.courseId = book.courseId;
    });
    function retrieveNodes() {
        $scope.newNodes = [];
        $scope.removedNodes = [];
        $scope.modifiedNodes = [];

        chapterService.getNodesByTextbookId($scope.tbId).then(function (response) {
            $scope.nodes = response.data;
            newNodeIndex = 1;
            initialNodes();
        })
    }

    $scope.goBack = function () {
        if (needToSave()) {
            $scope.$confirm("保存提醒", "您有未保存的数据，页面右下角有[保存]按钮哦！您忍心要离开吗？").then(function () {
                $scope.$goByUiSref($scope.returnUrl);
            });
        }
        else {
            $scope.$goByUiSref($scope.returnUrl);
        }
    }

    $scope.goToKPointMapping = function () {
        var stateName = "mdm.kpoint_mapping";
        var params = {
            tbId: $scope.tbId,
            tbName: $scope.tbName,
            courseId: $scope.courseId,
            returnUrl: $scope.returnUrl
        };
        if (needToSave()) {
            $scope.$confirm("保存", "您有未保存的数据，页面右下角有[保存]按钮哦！您忍心要离开吗？").then(function () {
                $state.go(stateName, params);
            });
        }
        else {
            $state.go(stateName, params);
        }
    }
    function needToSave() {
        collectChanges();
        if ($scope.newNodes.length == 0 && $scope.modifiedNodes.length == 0 && $scope.removedNodes.length == 0) {
            return false;
        }
        return true;
    }

    function initialNodes() {
        //增加根节点
        $scope.nodes.unshift({id: 0, parentId: "", name: $scope.tbName});

        //引导式学习,初始化一定的节点，用户就知道如何使用了
        if ($scope.nodes.length == 1) {
            $scope.$alertWarning("我们发现教材目录是空的，特意为您准备了一些示例章节，请参考！");
            //增加五个章
            for (var i = 1; i <= 3; i++) {
                $scope.nodes.push({
                    id: getNewNodeIndex(),
                    parentId: 0,
                    name: '新章' + i
                })
            }

            //第一章增加3个节
            for (var i = 1; i <= 3; i++) {
                var nodeId = getNewNodeIndex();
                $scope.nodes.push({
                    id: nodeId,
                    parentId: -1,
                    name: '新节' + i
                })

                if (i == 1) {
                    //第一个节增加1个小节
                    for (var j = 1; j <= 1; j++) {
                        $scope.nodes.push({
                            id: getNewNodeIndex(),
                            parentId: nodeId,
                            name: '新小节' + j
                        })
                    }
                }
            }
        }

        //展开所有的节点
        for (var idx in $scope.nodes) {
            $scope.nodes[idx].open = true;
        }
        $timeout(collectChanges, 300);
    }

    function getNewNodeIndex() {
        return -(newNodeIndex++);
    }

    //查询节点数据
    retrieveNodes();

    $scope.setting = {
        data: {
            key: {
                name: "name"
            },
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId"
            }
        },
        view: {
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom,
            selectedMulti: false,
            fontCss: getFontCss
        },
        edit: {
            enable: true,
            showRemoveBtn: showRemoveBtn,
            showRenameBtn: showRenameBtn,
            renameTitle: "重命名",
            removeTitle: "删除"
        },
        callback: {
            onRename: onRenameNode,
            onRemove: onRemoveNode,
            beforeEditName: beforeEditName,
            beforeRemove: beforeRemove,
            beforeRename: beforeRename,
            beforeDrag: beforeDrag,
            beforeDrop: beforeDrop,
            onDrop: onDropNode
        }
    }
    function getFontCss(treeId, treeNode) {
        if (parseInt(treeNode.id) < 0) {
            //新增
            return {color: "#5cb85c"};
        }
        else if (treeNode.changed) {
            //修改
            return {color: "#d6963b"};
        }

        return {color: "#333"};
    }

    function showRenameBtn(treeId, treeNode) {
        return treeNode.id != 0;
    }

    function beforeDrag(treeId, treeNodes) {
        for (var i = 0, l = treeNodes.length; i < l; i++) {
            if (treeNodes[i].id == 0) {
                return false;
            }
        }
        return true;
    }

    function beforeDrop(treeId, treeNodes, targetNode, moveType) {
        if (targetNode.id == 0 && moveType != "inner") return false;

        return targetNode ? targetNode.drop !== false : true;
    }

    function onDropNode() {
        $timeout(collectChanges);
    }

    function onRenameNode(event, treeId, treeNode, isCancel) {
        $timeout(collectChanges);
    }

    function onRemoveNode(event, treeId, treeNode) {
        if (parseInt(treeNode.id) > 0) {
            $scope.removedNodes.push(treeNode.id);

            if (treeNode.isParent) {
                var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
                //找到所有的子孙
                var allChildNodes = zTree.transformToArray(treeNode.children);
                angular.forEach(allChildNodes, function (node, key) {
                    //如果删除的是新节点，不记录
                    if (parseInt(treeNode.id) > 0)
                        $scope.removedNodes.push(node.id);
                })
            }
        }
        //删除新节点，也需要计算变化
        $timeout(collectChanges);
    }

    function beforeEditName(treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj(treeId);
        zTree.selectNode(treeNode);
        //TeckyLee，2016-06-03：不需要用户确认，直接进入
        // confirm("您确定要进入 【" + treeNode.name + "】 的编辑状态吗？");
        return true;
    }

    function beforeRemove(treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj(treeId);
        zTree.selectNode(treeNode);
        return confirm("您确认要删除 【" + treeNode.name + "】 吗？");
    }

    function showRemoveBtn(treeId, treeNode) {
        return treeNode.id != 0;
    }

    function beforeRename(treeId, treeNode, newName, isCancel) {
        newName = newName.trim();
        if (newName.length == 0) {
            alert("节点名称不能为空!");
            var zTree = $.fn.zTree.getZTreeObj(treeId);
            setTimeout(function () {
                zTree.editName(treeNode)
            }, 10);
            return false;
        }
        $timeout(collectChanges);
        return true;
    }

    function addHoverDom(treeId, treeNode) {
        var sObj = $("#" + treeNode.tId + "_span");
        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;
        var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
            + "' title='添加子节点' onfocus='this.blur();'></span>";
        sObj.after(addStr);

        var btn = $("#addBtn_" + treeNode.tId);
        if (btn) btn.bind("click", function () {
            var zTree = $.fn.zTree.getZTreeObj(treeId);
            var nodeName = "新";
            switch (treeNode.level) {
                case 0:
                    nodeName += "章";
                    break;
                case 1:
                    nodeName += "节";
                    break;
                default:
                    nodeName += "小节"
                    break;
            }
            nodeName += treeNode.children ? treeNode.children.length + 1 : 1;

            //新添加的node的id都是负数
            var newNodes = zTree.addNodes(treeNode, {
                id: (-newNodeIndex),
                pId: treeNode.id,
                name: nodeName
            });
            newNodeIndex++;

            setTimeout(function () {
                zTree.editName(newNodes[0]);
            }, 10);

            $timeout(collectChanges);
            return false;
        });
    }

    function removeHoverDom(treeId, treeNode) {
        $("#addBtn_" + treeNode.tId).unbind().remove();
    }

    //保存
    $scope.saveChanges = function () {
        collectChanges();
        if ($scope.newNodes.length == 0 && $scope.modifiedNodes.length == 0 && $scope.removedNodes.length == 0) {
            $scope.$alertWarning("目录树没有任何变化，无需保存！");
            return;
        }
        $scope.$confirm("保存确认", "请在变化统计中先确认您的修改。 您确定要保存对目录树的全部变化吗？").then(function () {

            var changes = {
                newNodes: $scope.newNodes,
                modifiedNodes: $scope.modifiedNodes,
                removedNodes: $scope.removedNodes
            };
            chapterService.saveCatalogChanges($scope.tbId, changes).then(function (response) {
                $scope.$alertInfo("保存成功！");
                retrieveNodes();
            });

        });
    }

    //恢复
    $scope.resume = function () {

        $scope.nodes = angular.copy($scope.nodes);
        $scope.newNodes = [];
        $scope.removedNodes = [];
        $scope.modifiedNodes = [];
        newNodeIndex = 1;
    }
    $scope.refreshChanges = collectChanges;

    //收集新增和修改的节点，删除的节点已经实时收集了
    function collectChanges() {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
        var nodes = zTree.transformToArray(zTree.getNodes());

        //清空一下两个数组
        $scope.newNodes = [];
        $scope.modifiedNodes = [];

        //收集新增或者修改的节点
        angular.forEach(nodes, function (node, key) {
            if (node.id == 0)
                return;

            if (parseInt(node.id) < 0) {
                //new node detected
                $scope.newNodes.push(getDataFromTreeNode(node));
            }
            else {
                //可能是update操作
                var original = findInOrginalNodes(node.id);
                var current = getDataFromTreeNode(node);
                var changes = getDiff(original, current);
                if (changes.length > 0) {
                    node.changed = true;
                    zTree.updateNode(node);
                    current.changes = changes;
                    $scope.modifiedNodes.push(current);
                }
                else {
                    if (node.changed) {
                        node.changed = false;
                        zTree.updateNode(node);
                    }
                    current.changes = null;
                }
            }
        });
    }

    function getDiff(node1, node2) {
        var changes = [];

        if (node1.name != node2.name) {
            changes.push("名");
        }

        if (node1.parentId != node2.parentId) {
            changes.push("父");
        }

        if (node1.ordinal != node2.ordinal) {
            changes.push("序");
        }

        return changes;
    }

    function getDataFromTreeNode(node) {
        return {
            id: node.id,
            name: node.name,
            parentId: node.getParentNode().id,
            textbookId: $scope.tbId,
            ordinal: node.getIndex()
        };
    }

    function findInOrginalNodes(id) {
        //没有找到的话，会返回undefined
        return $scope.nodes.find(function (item) {
            return item.id == id;
        })
    }

    $scope.getNameById = function (id) {
        var node = findInOrginalNodes(id);
        if (node) return node.name;
        return null;
    }

    $scope.export = function () {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
        var nodes = zTree.transformToArray(zTree.getNodes());
        var nodeNames = [];
        angular.forEach(nodes, function (node, index) {
            var row = [];
            row[parseInt(node.level)] = "[" + node.id + "] " + node.name;
            nodeNames.push(row);
        })
        csvExporter().exportJson(nodeNames, false, $scope.tbName + ' - 教材目录');
    }

}).service("chapterService", function ($http) {
    var pathTemplate = "$/textbooks/{tbId}/catalogs";

    var pathKPointsTemplate = "$/textbooks/{tbId}/catalogs/kpoints";
    var service = {};

    service.getNodesByTextbookId = function (tbId) {
        var realPath = pathTemplate.replace('{tbId}', tbId);
        return $http.get(realPath);
    }

    service.saveCatalogChanges = function (tbId, changes) {
        var realPath = pathTemplate.replace('{tbId}', tbId);
        return $http.put(realPath, changes);
    }

    service.getKPointsByTextbookId = function (tbId) {
        var realPath = pathKPointsTemplate.replace('{tbId}', tbId);
        return $http.get(realPath);
    }

    service.saveKPointChanges = function (tbId, changes) {
        var realPath = pathKPointsTemplate.replace('{tbId}', tbId);
        return $http.put(realPath, changes);
    }
    return service;
});

mdmApp.controller("mdm.course_index?stageId", function ($scope, $stateParams, $uibModal, $filter, courseService, stageService, subjectService) {
    $scope.stages = [];
    $scope.subjects = [];
    $scope.allCourses = [];
    $scope.params = {};


    //获取学段信息
    stageService.getStages().then(function (response) {
        $scope.stages = response.data;
        $scope.params.stageId = ($stateParams.stageId ? parseInt($stateParams.stageId) : 0);
    });

    //点击学段过滤列表
    $scope.search = function () {
        if ($scope.params.stageId == 0) {
            $scope.courses = $scope.allCourses;
            return;
        }
        $scope.courses = $filter('filter')($scope.allCourses, {"stageId": $scope.params.stageId});
    }
    $scope.getReturnUrl = function () {
        return "mdm.course_index({\"stageId\":" + $scope.params.stageId + "})";
    }
    //获取所有课程
    function showCourses() {
        courseService.getCourses().then(
            function (response) {
                $scope.courses = response.data;
                $scope.allCourses = response.data;
                $scope.search();
            });
    }

    //获取学科
    subjectService.getAll().then(function (data) {
        $scope.subjects = data;
    });

    showCourses();
    $scope.editMode = 'add';
    $scope.editCourse = {};
    var modalOptions = {
        templateUrl: 'views/mdm/course/edit.html',
        controller: 'mdm.course_index.edit',
        scope: $scope
    };

    //打开新增模态框
    $scope.openAddWindow = function () {
        $scope.editCourse = {};
        $scope.editMode = 'add';
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            $scope.allCourses.push(result);
            $scope.search();
        });
    };

    //打开更新窗口
    $scope.openEditWindow = function (course) {
        $scope.editMode = 'edit';
        $scope.editCourse = angular.copy(course);
        $uibModal.open(modalOptions).result.then(function (result) {
            var index = $scope.allCourses.indexOf(course);
            $scope.allCourses[index] = result;
            $scope.search();
        });
    };

    //打开设置教材版本窗口
    $scope.openSetVersionsWindow = function (course) {
        $scope.editCourse = angular.copy(course);
        $uibModal.open({
            templateUrl: 'views/mdm/course/setVersions.html',
            controller: 'mdm.course_index.setVersions',
            size: 'lg',
            resolve: {
                params: function () {
                    return {
                        course: angular.copy(course)
                    }
                }
            }
        });
    };

    //删除一个课程
    $scope.del = function (course) {
        var index = $scope.allCourses.indexOf(course);
        $scope.$confirm('删除课程', '您确定要删除课程[' + course.name + ']吗？').then(function (modalResult) {
            if (!modalResult) return;

            //执行删除
            courseService.deleteCourse(course).then(function (response) {
                if (response.data) {
                    $scope.$alertInfo("删除成功！");
                    //从当前列表中删除指定项
                    $scope.allCourses.splice(index, 1);
                    $scope.search();
                }
                else {
                    $scope.$alertError("删除失败！");
                }
            });
        });
    };

    //按学段和学科排序自动计算所有课程排序
    $scope.autoSort = function () {
        $scope.$confirm("重新排序", "将按照学段和学科的排序自动计算所有课程的排序，您确定要重新排序吗?").then(function () {
            courseService.autoSort().then(function (response) {
                $scope.allCourses = response.data;
            });
        });
    };
}).service('courseService', function ($http, dpService, $q) {
    var services = {};
    var resUrl = '$/courses';

    services.getCourses = function () {
        if (!services.courses) {
            return $http.get(resUrl).then(function (response) {
                services.courses = response.data;
                return {data: services.courses};
            });
        }
        var manualResponse = {data: services.courses};
        return $q.resolve(manualResponse);
    }

    services.getCourseById = function (courseId) {
        return services.getCourses().then(function (response) {
            var course = response.data.find(function (c) {
                return c.id == courseId;
            });
            return {data: course};
        });
    };

    //加上数据访问权限的课程获取函数
    services.getCoursesByDP = function (stageId) {
        return services.getCourses().then(function (response) {
            var courses = response.data.filter(function (c) {
                return c.stageId == stageId && dpService.allowed(c.stageId + ":" + c.id);
            });

            return {data: courses};
        });
    }

    services.addCourse = function (course) {
        return $http.post(resUrl, course);
    }

    services.updateCourse = function (course) {
        return $http.put(resUrl + "/" + course.id, course);
    }

    services.deleteCourse = function (course) {
        return $http.delete(resUrl + "/" + course.id);
    }

    /**
     * 设置课程关联的教材版本
     * @param courseId
     * @param versions versionIds数组
     * @returns {*}
     */
    services.setVersions = function (courseId, versions) {
        return $http.put(resUrl + "/" + courseId + "/textbookversions", versions);
    }

    services.getVersionsByCourseId = function (courseId) {
        return $http.get(resUrl + "/" + courseId + "/textbookversions");
    }

    /**
     * 课程自动排序
     */
    services.autoSort = function () {
        return $http.put(resUrl + "/autosort");
    }

    return services;
}).controller('mdm.course_index.edit', ['$filter', '$scope', 'courseService', function ($filter, $scope, courseService) {

    $scope.submitForm = function () {
        if (!$scope.courseEditForm.$valid) {
            return;
        }
        //提交时，根据编辑模式选择调用不同的service方法
        if ($scope.editMode == 'add') {
            courseService.addCourse($scope.editCourse).then(saveOnSuccess);
        }
        else {
            courseService.updateCourse($scope.editCourse).then(saveOnSuccess);
        }
    };

    $scope.updateChange = function () {
        var stageName = "";
        var subjectName = "";
        if ($scope.editCourse.stageId) {
            stageName = $filter('nameOfId')($scope.editCourse.stageId, $scope.stages);
        }
        if ($scope.editCourse.subjectId) {
            subjectName = $filter('nameOfId')($scope.editCourse.subjectId, $scope.subjects);
        }
        $scope.editCourse.name = stageName + subjectName;
    };
    function saveOnSuccess(response) {
        $scope.$alertInfo("保存成功！");

        //关闭窗口，并设置modal的result
        $scope.$close($scope.editCourse);
    }
}]).filter("nameOfId", function () {
        return function (input, list) {
            var element = list.find(function (element) {
                return element.id == input;
            });
            return element && element.name;
        }
    })
    .controller("mdm.course_index.setVersions", function ($scope, textbookVersionService, courseService, params) {
        $scope.course = params.course;
        $scope.versions = [];
        textbookVersionService.getTextbookVersionsByCourseId($scope.course.id).then(function (resposne) {
            $scope.course.versions = resposne.data;
        }).then(function () {
            textbookVersionService.getTextbookVersions($scope.course.stageId).then(function (response) {
                $scope.versions = response.data;
                $scope.versions.forEach(function (version) {
                    //把课程包含的教材版本都置为选中状态
                    version.checked = $scope.course.versions.some(function (item) {
                        return item.id == version.id;
                    })
                })
            });
        });

        //选择课程过滤器
        $scope.selectedVersion = function (item) {
            return !!item.checked
        }

        $scope.versionKeyWord = '';
        $scope.listVersion = function (item) {
            return (!item.checked) && item.name.indexOf($scope.versionKeyWord) >= 0

        }
        $scope.changeVersion = function (version) {
            version.checked = !version.checked
        }


        $scope.submitForm = function () {
            var selectedVersions = $scope.versions.filter(function (item) {
                return !!item.checked;
            });
            var selectedVersionIds = selectedVersions.map(function (item) {
                return item.id;
            });
            courseService.setVersions($scope.course.id, selectedVersionIds).then(function (response) {
                if (response.data) {
                    $scope.course.versions = selectedVersions;
                    $scope.$close($scope.course);
                    $scope.$alertInfo("设置教材版本成功");
                } else {
                    $scope.$alertError("设置教材版本失败");
                }
            });
        }
    });

/**
 * @author fupengfei
 * @since 1.0
 */
angular.module('mdmApp')
    .service('gradeService', function ($http, $q) {
        var url = '$/grades';
        var services = {};
        var allGrades = null;
        //获取年级列表服务,返回Promise
        services.getGrades = function (noCache) {
            if (!noCache && allGrades)
                return $q.resolve({data: allGrades});

            return $http.get(url).then(function (response) {
                allGrades = response.data;
                return response;
            });
        }

        //新增年级服务
        services.editGrade = function (mode, formData) {
            if (mode == "add") {
                return $http.post(url, formData);
            }
            return $http.put(url, formData);
        }

        services.delGrade = function (gradeId) {
            var delUrl = url + '/' + gradeId;
            return $http.delete(delUrl);
        }

        services.getGradesByStageId = function (stageId) {
            return services.getGrades().then(function (response) {
                var grades = response.data;

                switch (stageId) {
                    case 2:
                        //小学
                        grades = grades.filter(isPrimaryGrade);
                        break;
                    case 3:
                        //初中
                        grades = grades.filter(isMiddleGrade);
                        break;
                    case 4:
                        //高中
                        grades = grades.filter(isHighGrade);
                        break;
                    default:
                        grades = [];
                        break;
                }
                return {data: grades};
            });
        }

        function isHighGrade(g) {
            return g.id >= 10 && g.id <= 12
        }

        function isMiddleGrade(g) {
            return g.id >= 6 && g.id <= 9
        }

        function isPrimaryGrade(g) {
            return g.id <= 6;
        }

        return services;
    });

/**
 * Created by Administrator on 2016/4/27.
 */
angular.module('mdmApp')
    .controller('mdm.grade_index', ['$log', '$scope', '$uibModal', 'gradeService', '$stateParams', 'appConstants', function ($log, $scope, $uibModal, gradeService, $stateParams, appConstants) {

        //获取年级列表
        $scope.listGrades = function () {
            gradeService.getGrades(true).then(function (response) {
                $scope.grades = response.data;
            });
        }
        $scope.listGrades();
        //页码切换
        $scope.pageChanged = function (pageNo) {
            $log.info(pageNo);
            $scope.currentPage = pageNo;
            $scope.listGrades();
        }
        //编辑年级模态框
        $scope.edit = function (grade) {
            $scope.mode = "add";
            $scope.grade = {};
            if (grade) {
                $scope.mode = "edit";
                $scope.grade = angular.copy(grade);
            }
            showModal();
        };
        //删除年级
        $scope.delete = function (grade) {
            $scope.$confirm('删除年级', '警告！此操作不可恢复。您确定要删除年级 [' + grade.name + ']吗？').then(function (modalResult) {
                if (!modalResult) return;
                //执行删除
                gradeService.delGrade(grade.id).then(function (response) {
                    if (response.data) {
                        $scope.$alertInfo("删除成功！");
                        $scope.listGrades();
                    } else {
                        $scope.$alertError('删除失败！');
                    }
                })
            });
        };
        //学段下拉框model初始化
        $scope.stageOptions = appConstants.stageOptions;
        //弹出编辑模态框
        function showModal() {
            $uibModal.open({
                templateUrl: 'views/mdm/grade/edit.html',
                controller: 'mdm.grade_index.edit',
                scope: $scope
            });
        }

    }]).controller('mdm.grade_index.edit', ['$log', '$scope', 'gradeService', function ($log, $scope, gradeService) {
    //表单提交
    $scope.submitForm = function () {
        if ($scope.gradeForm.$valid) {
            gradeService.editGrade($scope.mode, $scope.grade).then(function (response) {
                if (response.data) {
                    $scope.$alertInfo("保存成功!");
                    $scope.$close();
                    $scope.listGrades();
                }
            })
        }

    };
}]);

/**
 * Created by admin on 2016/5/2.
 */
mdmApp.controller("mdm.home", function ($scope, userService, logService, dateService) {
    userService.getCurrentUser().then(function (response) {
        $scope.loginUser = response.data;
    });
    $scope.params = {};
    $scope.params.currentpage = 1;//params作为url参数都要小写
    $scope.params.pagesize = 10;

    var now = new Date();
    $scope.params.starttime = dateService.dateAdd('m', -3, angular.copy(now));
    $scope.params.endtime = now;
    $scope.params.type = 'DATA';
    logService.getLogs($scope.params).then(function (response) {
        $scope.logs = response.data.items;
    });
});

/**
 * Created by TeckyLee on 2016/6/6.
 */
//tbid = course id
//tbname = course name
mdmApp.controller('mdm.kpoint_edit?courseId&courseName&returnUrl', function ($scope, $stateParams, kpointService, $timeout) {
    $scope.courseId = $stateParams.courseId;
    $scope.courseName = $stateParams.courseName;
    $scope.ztreeId = "mdm_kpoint_edit";
    $scope.nodes = [];
    $scope.returnUrl = $stateParams.returnUrl ? $stateParams.returnUrl : "mdm.kpoint_index";

    var newNodeIndex = 1;

    function retrieveNodes() {
        $scope.newNodes = [];
        $scope.removedNodes = [];
        $scope.modifiedNodes = [];

        kpointService.getNodesByCourseId($scope.courseId).then(function (response) {
            $scope.nodes = response.data;
            newNodeIndex = 1;
            //增加根节点
            $scope.nodes.unshift({id: 0, parentId: "", name: $scope.courseName});

            initialNodes();

            //展开所有的节点
            for (var idx in $scope.nodes) {
                //因为知识树太长，所以只展开第一级即可
                if ($scope.nodes[idx].parentId == 0)
                    $scope.nodes[idx].open = true;

            }
            $timeout(collectChanges, 300);
        })
    }

    function initialNodes() {


        //引导式学习,初始化一定的节点，用户就知道如何使用了
        if ($scope.nodes.length == 1) {
            $scope.$alertWarning("我们发现知识树是空的，特意为您准备了一些示例节点，请参考！");
            //增加五个章
            for (var i = 1; i <= 3; i++) {
                $scope.nodes.push({
                    id: getNewNodeIndex(),
                    parentId: 0,
                    name: '一级知识点' + i
                })
            }

            //第一章增加3个节
            for (var i = 1; i <= 3; i++) {
                var nodeId = getNewNodeIndex();
                $scope.nodes.push({
                    id: nodeId,
                    parentId: -1,
                    name: '二级知识点' + i
                })

                if (i == 1) {
                    //第一个节增加1个小节
                    for (var j = 1; j <= 1; j++) {
                        $scope.nodes.push({
                            id: getNewNodeIndex(),
                            parentId: nodeId,
                            name: '三级知识点' + j
                        })
                    }
                }
            }
        }
    }

    function getNewNodeIndex() {
        return -(newNodeIndex++);
    }

    //查询节点数据
    retrieveNodes();

    $scope.setting = {
        data: {
            key: {
                name: "name"
            },
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId"
            }
        },
        view: {
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom,
            selectedMulti: false,
            fontCss: getFontCss,
            addDiyDom: addDiyDom
        },
        edit: {
            enable: true,
            showRemoveBtn: showRemoveBtn,
            showRenameBtn: showRenameBtn,
            renameTitle: "重命名",
            removeTitle: "删除"
        },
        callback: {
            onRename: onRenameNode,
            onRemove: onRemoveNode,
            beforeEditName: beforeEditName,
            beforeRemove: beforeRemove,
            beforeRename: beforeRename,
            beforeDrag: beforeDrag,
            beforeDrop: beforeDrop,
            onDrop: onDropNode
        }
    }

    function addDiyDom(treeId, treeNode) {
        if (!(treeNode.type)) treeNode.type = "NODE";
        var target = $("#" + treeNode.tId + '_ico');

        var selectBox = $("#" + treeNode.tId + '_a').find(".type-select-box");
        if (selectBox.length > 0) {
            selectBox.remove();
        }

        var typeText = "";
        var typeClass = "";
        switch (treeNode.type) {
            case "KNOWLEDGE_POINT":
                typeText = "知";
                typeClass = "warning";
                break;
            case "TESTING_POINT":
                typeText = "考";
                typeClass = "primary";
                break;
            default:
                typeText = "节";
                typeClass = "normal";
                break;
        }
        var selectBoxHTML = '<span class="type-select-box">\
            <span type="button" class="badge bottom-buttons ' + typeClass + '" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' + typeText + '</span>\
            <ul class="dropdown-menu">\
                <li><a enum-value="NODE" href="#">节点</a></li>\
                <li><a enum-value="KNOWLEDGE_POINT" href="#">知识点</a></li>\
                <li><a enum-value="TESTING_POINT" href="#">考点</a></li>\
            </ul>\
        </span>';
        target.after(selectBoxHTML);
        selectBox = target.parent();

        var badge = selectBox.find('.badge');
        selectBox.find('.dropdown-menu a').bind('click', function (e) {
            e.preventDefault();
            treeNode.type = $(this).attr('enum-value');
            addDiyDom(treeId, treeNode);
            $timeout(collectChanges);
        });
    }

    function getFontCss(treeId, treeNode) {
        if (parseInt(treeNode.id) < 0) {
            //新增
            return {color: "#5cb85c"};
        }
        else if (treeNode.changed) {
            //修改
            return {color: "#d6963b"};
        }

        return {color: "#333"};
    }

    function showRenameBtn(treeId, treeNode) {
        return treeNode.id != 0;
    }

    function beforeDrag(treeId, treeNodes) {
        for (var i = 0, l = treeNodes.length; i < l; i++) {
            if (treeNodes[i].id == 0) {
                return false;
            }
        }
        return true;
    }

    function beforeDrop(treeId, treeNodes, targetNode, moveType) {
        if (targetNode.id == 0 && moveType != "inner") return false;

        return targetNode ? targetNode.drop !== false : true;
    }

    function onDropNode() {
        $timeout(collectChanges);
    }

    function onRenameNode(event, treeId, treeNode, isCancel) {
        $timeout(collectChanges);
    }

    function onRemoveNode(event, treeId, treeNode) {
        if (parseInt(treeNode.id) > 0) {
            $scope.removedNodes.push(treeNode.id);

            if (treeNode.isParent) {
                var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
                //找到所有的子孙
                var allChildNodes = zTree.transformToArray(treeNode.children);
                angular.forEach(allChildNodes, function (node, key) {
                    //如果删除的是新节点，不记录
                    if (parseInt(treeNode.id) > 0)
                        $scope.removedNodes.push(node.id);
                })
            }
        }
        //删除新节点，也需要计算变化
        $timeout(collectChanges);
    }

    function beforeEditName(treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj(treeId);
        zTree.selectNode(treeNode);
        //TeckyLee，2016-06-03：不需要用户确认，直接进入
        // confirm("您确定要进入 【" + treeNode.name + "】 的编辑状态吗？");
        return true;
    }

    function beforeRemove(treeId, treeNode) {
        var zTree = $.fn.zTree.getZTreeObj(treeId);
        zTree.selectNode(treeNode);
        return confirm("您确认要删除 【" + treeNode.name + "】 吗？");
    }

    function showRemoveBtn(treeId, treeNode) {
        return treeNode.id != 0;
    }

    function beforeRename(treeId, treeNode, newName, isCancel) {
        newName = newName.trim();
        if (newName.length == 0) {
            alert("节点名称不能为空!");
            var zTree = $.fn.zTree.getZTreeObj(treeId);
            setTimeout(function () {
                zTree.editName(treeNode)
            }, 10);
            return false;
        }
        $timeout(collectChanges);
        return true;
    }

    function addHoverDom(treeId, treeNode) {
        var sObj = $("#" + treeNode.tId + "_span");
        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;
        var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
            + "' title='添加子节点' onfocus='this.blur();'></span>";
        sObj.after(addStr);

        var btn = $("#addBtn_" + treeNode.tId);
        if (btn) btn.bind("click", function () {
            var zTree = $.fn.zTree.getZTreeObj(treeId);
            var nodeName = "新知识点";

            nodeName += treeNode.children ? treeNode.children.length + 1 : 1;

            //新添加的node的id都是负数
            var newNodes = zTree.addNodes(treeNode, {
                id: (-newNodeIndex),
                pId: treeNode.id,
                name: nodeName
            });
            newNodeIndex++;

            setTimeout(function () {
                zTree.editName(newNodes[0]);
            }, 10);

            $timeout(collectChanges);
            return false;
        });
    }

    function removeHoverDom(treeId, treeNode) {
        $("#addBtn_" + treeNode.tId).unbind().remove();
    }

    $scope.recognize = function () {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
        var nodes = zTree.transformToArray(zTree.getNodes());
        angular.forEach(nodes, function (node) {
            if (!node.children || node.children.length == 0) {
                if (node.type == "NODE") {
                    //is leaf
                    node.type = "TESTING_POINT";
                    addDiyDom($scope.ztreeId, node);
                    var parentNode = node.getParentNode();
                    parentNode.type = "KNOWLEDGE_POINT";
                    addDiyDom($scope.ztreeId, parentNode);
                }
            }
        });

        collectChanges();
    }

    $scope.goBack = function () {
        collectChanges();
        if ($scope.newNodes.length == 0 && $scope.modifiedNodes.length == 0 && $scope.removedNodes.length == 0) {
            $scope.$goByUiSref($scope.returnUrl);
            return;
        }

        $scope.$confirm("保存提醒", "您有未保存的数据，页面右下角有[保存]按钮哦！您忍心要离开吗？").then(function () {
            $scope.$goByUiSref($scope.returnUrl);
        });
    };
    
    //保存
    $scope.saveChanges = function () {
        collectChanges();
        if ($scope.newNodes.length == 0 && $scope.modifiedNodes.length == 0 && $scope.removedNodes.length == 0) {
            $scope.$alertWarning("知识树没有任何变化，无需保存！");
            return;
        }
        $scope.$confirm("保存确认", "请在变化统计中先确认您的修改。 您确定要保存对知识树的全部更改吗？").then(function () {

            var changes = {
                newNodes: $scope.newNodes,
                modifiedNodes: $scope.modifiedNodes,
                removedNodes: $scope.removedNodes
            };
            kpointService.saveChanges($scope.courseId, changes).then(function (response) {
                $scope.$alertInfo("保存成功！");
                retrieveNodes();
            });

        });
    }

    //恢复
    $scope.resume = function () {

        $scope.nodes = angular.copy($scope.nodes);
        $scope.newNodes = [];
        $scope.removedNodes = [];
        $scope.modifiedNodes = [];
        newNodeIndex = 1;
    }
    $scope.refreshChanges = collectChanges;

    //收集新增和修改的节点，删除的节点已经实时收集了
    function collectChanges() {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
        var nodes = zTree.transformToArray(zTree.getNodes());

        //清空一下两个数组
        $scope.newNodes = [];
        $scope.modifiedNodes = [];

        //收集新增或者修改的节点
        angular.forEach(nodes, function (node, key) {
            if (node.id == 0)
                return;

            if (parseInt(node.id) < 0) {
                //new node detected
                $scope.newNodes.push(getDataFromTreeNode(node));
            }
            else {
                //可能是update操作
                var original = findInOrginalNodes(node.id);
                var current = getDataFromTreeNode(node);
                console.log(current);
                var changes = getDiff(original, current);
                if (changes.length > 0) {
                    node.changed = true;
                    zTree.updateNode(node);

                    current.changes = changes;
                    $scope.modifiedNodes.push(current);
                }
                else {
                    if (node.changed) {
                        node.changed = false;
                        zTree.updateNode(node);
                    }
                    current.changes = null;
                }
            }
        });
    }

    function getDiff(node1, node2) {
        var changes = [];

        if (node1.name != node2.name) {
            changes.push("名");
        }

        if (node1.parentId != node2.parentId) {
            changes.push("父");
        }

        if (node1.ordinal != node2.ordinal) {
            changes.push("序");
        }

        if (node1.type != node2.type) {
            changes.push("类");
            console.log(node2.type);
        }
        return changes;
    }

    function getDataFromTreeNode(node) {
        return {
            id: node.id,
            name: node.name,
            type: node.type,
            parentId: node.getParentNode().id,
            courseId: $scope.courseId,
            ordinal: node.getIndex()
        };
    }

    function findInOrginalNodes(id) {
        //没有找到的话，会返回undefined
        return $scope.nodes.find(function (item) {
            return item.id == id;
        })
    }

    $scope.getNameById = function (id) {
        var node = findInOrginalNodes(id);
        if (node) return node.name;
        return null;
    }

    $scope.export = function () {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeId);
        var nodes = zTree.transformToArray(zTree.getNodes());
        var nodeNames = [];
        angular.forEach(nodes, function (node, index) {
            var row = [];
            row[parseInt(node.level)] = "[" + node.id + "] " + node.name;
            nodeNames.push(row);
        });
        csvExporter().exportJson(nodeNames, false, $scope.courseName + ' - 知识树');
    }
})

/**
 * Created by TeckyLee on 2016/6/6.
 */
mdmApp.controller("mdm.kpoint_index", function ($scope, stageService, courseService) {
    stageService.getStagesByDP().then(function (response) {
        $scope.stages = response.data;
        $scope.courses = {};
        angular.forEach($scope.stages, function (stage, key) {
            courseService.getCoursesByDP(stage.id).then(function (response) {
                $scope.courses[stage.id] = response.data;
            });
        });
    });
    $scope.button_classes = ['info', 'warning', 'success', 'danger', 'primary'];
}).service("kpointService", function ($http) {
    var pathTemplate = "$/courses/{courseId}/kpoints";

    var service = {};

    service.getNodesByCourseId = function (courseId) {
        var realPath = pathTemplate.replace('{courseId}', courseId);
        return $http.get(realPath);
    }

    service.saveChanges = function (courseId, changes) {
        var realPath = pathTemplate.replace('{courseId}', courseId);
        return $http.put(realPath, changes);
    }
    return service;
});

mdmApp
    .controller('mdm.log_index', ['$rootScope', '$scope', 'appConstants', 'logService', 'dateService', function ($rootScope, $scope, appConstants, logService, dateService) {
        $scope.params = {};
        $scope.params.currentpage = 1;//params作为url参数都要小写
        $scope.params.pagesize = appConstants.pageSize;
        $scope.totalSize = 0;
        $scope.totalPage = 0;
        //选择不同日期范围，计算开始和结束时间
        $scope.computeTime = function () {
            var now = new Date();

            switch ($scope.selectedDateRange) {
                case 1:
                    $scope.params.starttime = dateService.dateAdd('w', -1, angular.copy(now));
                    $scope.params.endtime = now;
                    break;
                case 2:
                    $scope.params.starttime = dateService.dateAdd('m', -1, angular.copy(now));
                    $scope.params.endtime = now;
                    break;
                case 3:
                    $scope.params.starttime = dateService.dateAdd('m', -3, angular.copy(now));
                    $scope.params.endtime = now;
                    break;
                case 4:
                    $scope.params.starttime = "";
                    $scope.params.endtime = ""
                    break;
            }
        }
        //日期范围下拉框数据
        $scope.dateRanges = [
            {id: 1, text: "最近一周"},
            {id: 2, text: "最近一个月"},
            {id: 3, text: "最近三个月"},
            {id: 4, text: "自定义时间"}
        ];
        //日志类型下拉框数据
        $scope.logTypes = [
            {id: "DATA", name: "数据"},
            {id: "USER", name: "用户"},
            {id: "SYS", name: "系统"}
        ];
        //默认选中最近一周
        $scope.selectedDateRange = 1;
        $scope.computeTime();
        // $scope.customDate = false;
        //控制日期控件的显示与隐藏
        $scope.st = {
            opened: false,
            open: function () {
                this.opened = true;
            }
        };
        $scope.et = {
            opened: false,
            open: function () {
                this.opened = true;
            }
        };
        //日志列表
        $scope.searchLogs = function (keyEvent) {
            if (keyEvent && keyEvent.which != 13) {
                return;
            }
            if (!$scope.params.starttime) {
                $scope.params.starttime = new Date();
            }
            if (!$scope.params.endtime) {
                $scope.params.endtime = new Date();
            }
            $scope.params.starttime.setHours(0, 0, 0, 0);
            $scope.params.endtime.setHours(23, 59, 59, 999);
            if ($scope.params.starttime > $scope.params.endtime) {
                $rootScope.$alertWarning("开始时间不能晚于结束时间");
                return;
            }
            logService.getLogs($scope.params).then(function (response) {
                $scope.logs = response.data.items;
                $scope.params.currentpage = response.data.currentPage;
                $scope.params.pagesize = response.data.pageSize;
                $scope.totalSize = response.data.totalSize;
                $scope.totalPage = response.data.totalPage;
            });
        }

        //页码切换
        $scope.pageChanged = function (pageNo) {
            $scope.params.currentpage = pageNo;
            $scope.searchLogs();
        }
        $scope.searchLogs();
    }])
    .service("logService", ["$http", function ($http) {
        var logSrv = {};
        var logUrl = "$/logs";

        logSrv.getLogs = function (params) {
            return $http.get(logUrl, {"params": params});
        }
        return logSrv;
    }]).service('dateService', function () {
    var services = {};
    services.dateAdd = function (interval, number, date) {
        switch (interval) {
            case "y":
            {
                date.setFullYear(date.getFullYear() + number);
                return date;
                break;
            }
            case "q":
            {
                date.setMonth(date.getMonth() + number * 3);
                return date;
                break;
            }
            case "m":
            {
                date.setMonth(date.getMonth() + number);
                return date;
                break;
            }
            case "w":
            {
                date.setDate(date.getDate() + number * 7);
                return date;
                break;
            }
            case "d":
            {
                date.setDate(date.getDate() + number);
                return date;
                break;
            }
            case "h":
            {
                date.setHours(date.getHours() + number);
                return date;
                break;
            }
            case "m":
            {
                date.setMinutes(date.getMinutes() + number);
                return date;
                break;
            }
            case "s":
            {
                date.setSeconds(date.getSeconds() + number);
                return date;
                break;
            }
            default:
            {
                date.setDate(d.getDate() + number);
                return date;
                break;
            }
        }
    }
    return services;

});

/**
 * Created by 惠芳 on 5/2/2016.
 */
mdmApp
    .controller('mdm.org_index', ['$log', '$scope', '$uibModal', '$q', 'appConstants', 'stageService', 'orgService', function ($log, $scope, $uibModal, $q, appConstants, stageService, orgService) {
        $scope.params = {};
        $scope.params.currentpage = 1;//params作为url参数都要小写
        $scope.params.pagesize = appConstants.pageSize;
        $scope.totalSize = 0;
        $scope.totalPage = 0;

        $scope.searchOrgs = function (keyEvent) {
            if (keyEvent && keyEvent.which != 13) {
                return;
            }
            orgService.getOrgs($scope.params).then(function (response) {
                $scope.orgs = response.data.items;
                $scope.params.currentpage = response.data.currentPage;
                $scope.params.pagesize = response.data.pageSize;
                $scope.totalSize = response.data.totalSize;
                $scope.totalPage = response.data.totalPage;
            });
        }

        //先异步获取 orgTypes,stages数据，完成后在执行searchOrgs。因为org数据绑定的时候依赖orgTypes,stages数据
        $q.all([orgService.getOrgTypes().then(function (response) {
            $scope.orgTypes = response.data;
        }),
            stageService.getStages().then(function (response) {
                $scope.stages = response.data;
            })]).then(function () {
            $scope.searchOrgs();
        });

        //根据orgTypeId解析名称
        $scope.getOrgTypeName = function (orgTypeId) {
            var orgType = $scope.orgTypes.find(function (i) {
                return i.id == orgTypeId
            });
            return orgType && orgType.name;
        }

        //页码切换
        $scope.pageChanged = function (pageNo) {
            $scope.params.currentpage = pageNo;
            $scope.searchOrgs();
        }

        $scope.openEditOrg = function (editOrg) {
            $scope.org = angular.copy(editOrg);
            $uibModal.open({
                templateUrl: 'views/mdm/org/edit.html',
                scope: $scope,
                backdrop: "static"
            });
        }
    }])
    .service("orgService", ["$http", "$q", function ($http, $q) {
        var orgSrv = {};
        var orgUrl = "$/orgs";
        var orgTypeUrl = "$/orgtypes";

        orgSrv.getOrgs = function (params) {
            return $http.get(orgUrl, {"params": params});
        }

        /**
         * 获取所有出版社
         * @returns {返回结果是出版社数组}
         */
        orgSrv.getAllPress = function () {
            return $http.get(orgUrl, {"params": {"typeid": 8}}).then(function (response) {
                return response.data.items;
            });
        }

        orgSrv.getOrgTypes = function () {
            if (orgSrv.orgTypes) {
                var manualResponse = {data: orgSrv.orgTypes};
                return $q.resolve(manualResponse);
            }
            var orgTypesPromise = $http.get(orgTypeUrl);
            orgTypesPromise.then(function (response) {
                orgSrv.orgTypes = response.data;
            });
            return orgTypesPromise;
        }

        return orgSrv;
    }])
;

mdmApp.controller("mdm.permission_index?appId&appName", ['$scope', 'permissionService', 'applicationService', '$stateParams', function ($scope, permissionService, applicationService, $stateParams) {

    $scope.ztreeId = "permission_index";
    $scope.settings = {
        data: {
            simpleData: {
                enable: true,
                idKey: "id",
                pIdKey: "parentId"
            }
        }
    };
    $scope.appId = $stateParams.appId;
    $scope.appName = $stateParams.appName;

    var param = {appId: $scope.appId};
    permissionService.getPermissionTrees(param).then(function (response) {
        $scope.permissionTree = response.data;

        //全部设置为打开状态
        angular.forEach($scope.permissionTree, function (value, key) {
            value.open = true;
            value.name = value.name + " - " + value.value;
        });
    });

    $scope.export = function () {
        csvExporter().exportJson($scope.permissionTree, true, $scope.appName + '的权限定义树');
    }

}]).service("permissionService", ['$http', function ($http) {
    var services = {};
    var url = '$/permissions';
    services.getPermissionTrees = function (params) {
        return $http.get(url, {"params": params});
    }
    return services;
}]);

/**
 * Created by TeckyLee on 2016/6/2.
 */
mdmApp.controller('mdm.role_index.data_permission', function ($log, $scope, $q, stageService, courseService, permissionService, roleService) {
    var stages = [];
    var courses = [];

    var promises = [];
    var stagePromise = stageService.getStages().then(function (response) {
        stages = response.data;
    });
    promises.push(stagePromise);

    var coursePromise = courseService.getCourses().then(function (response) {
        courses = response.data;
    });
    promises.push(coursePromise);

    $q.all(promises).then(function () {
        //创建数据权限定义树
        if (!$scope.permissionTree) {
            createDataPermissionTree();
        }

        //获取当前角色的权限
        roleService.getPermissions($scope.editRole.id).then(function (response) {
            $scope.exps = response.data;
        });
    });

    function createDataPermissionTree() {
        var tree = [];

        //根节点
        tree.push({
            id: 0,
            name: $scope.appName + "数据",
            parentId: "",
            value: $scope.appId + ".data",
            open: true
        });

        //获取权限定义树
        angular.forEach(stages, function (stage, idx) {

            var stageNodeId = 's' + stage.id;
            tree.push({
                id: stageNodeId,
                name: stage.name,
                parentId: 0,
                value: stage.id + "",
                open: true
            });

            var subCourses = courses.filter(function (c) {
                return c.stageId == stage.id;
            });

            angular.forEach(subCourses, function (course, idx) {
                var courseNodeId = 'c' + course.id;
                tree.push({
                    id: courseNodeId,
                    name: course.name.replace(stage.name, ''),
                    parentId: stageNodeId,
                    value: course.id + ""
                });
            });
        });

        $scope.permissionTree = tree;
    }

    $scope.submitForm = function () {
        if ($scope.exps.length == 0) {
            $scope.$alertError("权限不能为空！");
            return;
        }

        roleService.setPermissions($scope.editRole.id, $scope.exps).then(saveOnSuccess);
    }

    function saveOnSuccess(response) {
        $scope.$alertInfo("保存成功！");

        //关闭窗口
        $scope.$close();
    }
});

/**
 * Created by tecky lee on 2016/5/2.
 */
mdmApp.controller("mdm.role_index?appId&appName&roleType", function ($log, $scope, $uibModal, roleService, applicationService, $stateParams) {
    $scope.appName = $stateParams.appName;
    $scope.appId = $stateParams.appId;
    $scope.roleType = $stateParams.roleType;

    roleService.getRoles($scope.appId, $scope.roleType).then(
        function (response) {
            $scope.roles = response.data;
        });

    $scope.editMode = 'add';
    $scope.editRole = {};

    var modalOptions = {
        templateUrl: 'views/mdm/role/edit.html',
        controller: 'mdm.role_index.edit',
        scope: $scope
    };

    //打开新增模态框
    $scope.openAddWindow = function () {
        $scope.editRole = {type: $scope.roleType};
        $scope.editMode = 'add';
        $scope.editRole.applicationId = $scope.appId;
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            $scope.roles.push(result);
            //紧接着继续设置权限
            $scope.openPermissionWindow(result);
        });
    };

    //打开更新窗口
    $scope.openEditWindow = function (role) {
        $scope.editMode = 'edit';
        $scope.editRole = angular.copy(role);
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            angular.copy(result, role);
        });
    };

    $scope.getEntityName = function () {
        return $scope.roleType == 'MENU' ? '角色' : '数据范围';
    }

    //打开权限编辑窗口
    $scope.openPermissionWindow = function (role) {
        $scope.editRole = angular.copy(role);
        var modalOption = {
            templateUrl: 'views/mdm/role/edit_permission.html',
            controller: ($scope.roleType == 'MENU' ? 'mdm.role_index.menu_permission' : 'mdm.role_index.data_permission'),
            scope: $scope,
            size: "lg",
            backdrop: "static"
        };

        $uibModal.open(modalOption);
    };

    $scope.trim = function (role) {
        role.id = role.id.trim();
        role.name = role.name.trim();
    }

    //删除一个角色
    $scope.del = function (role) {
        var index = $scope.roles.indexOf(role);
        $scope.$confirm('删除角色', '您确定要删除[' + role.name + ']吗？').then(function (modalResult) {
            if (!modalResult) return;

            //执行删除
            roleService.deleteRole(role.id).then(function (response) {
                if (response.data) {
                    $scope.$alertInfo("删除成功！");
                    //从当前列表中删除指定项
                    $scope.roles.splice(index, 1);
                }
                else {
                    $scope.$alertError("删除失败！");
                }
            });
        });
    };
}).service('roleService', ['$http', function ($http) {
    var services = {};
    var resUrl = '$/roles';

    services.getRoles = function (appId, roleType) {
        return $http.get("$/applications/" + appId + "/" + roleType + "/roles/");
    }

    services.addRole = function (role) {
        return $http.post(resUrl, role);
    }

    services.updateRole = function (role) {
        return $http.put(resUrl + "/" + role.id, role);
    }

    services.deleteRole = function (roleId) {
        return $http.delete(resUrl + "/" + roleId);
    }

    services.getPermissions = function (roleId) {
        return $http.get(resUrl + "/" + roleId + "/permissions");
    }

    services.setPermissions = function (roleId, permissions) {
        return $http.post(resUrl + "/" + roleId + "/permissions", permissions);
    }

    services.getFlakeId = function () {
        return $http.get(resUrl + "/flakeid");
    }

    return services;
}]).controller('mdm.role_index.edit', ['$log', '$scope', 'roleService', function ($log, $scope, roleService) {
    if (isAddMode()) {
        roleService.getFlakeId().then(function (response) {
            $scope.editRole.id = response.data;
        });
    }

    $scope.submitForm = function () {
        if (!$scope.roleEditForm.$valid) return;

        //trim string fields
        $scope.trim($scope.editRole);

        //提交时，根据编辑模式选择调用不同的service方法
        if (isAddMode()) {
            roleService.addRole($scope.editRole).then(saveOnSuccess);
        }
        else {
            roleService.updateRole($scope.editRole).then(saveOnSuccess);
        }
    }

    function isAddMode() {
        return $scope.editMode == 'add';
    }

    function saveOnSuccess(response) {
        $scope.$alertInfo("保存成功！");

        //关闭窗口，并设置modal的result
        $scope.$close($scope.editRole);
    }
}]);

/**
 * Created by TeckyLee on 2016/6/2.
 */
mdmApp.controller('mdm.role_index.menu_permission', function ($log, $scope, permissionService, roleService) {

    //获取权限定义树
    permissionService.getPermissionTrees({appId: $scope.editRole.applicationId}).then(function (response) {
        $scope.permissionTree = response.data;
        //全部设置为打开状态
        angular.forEach($scope.permissionTree, function (value, key) {
            value.open = true;
        });
    });

    //获取当前role的权限列表
    roleService.getPermissions($scope.editRole.id).then(function (response) {
        $scope.exps = response.data;
    });

    $scope.exps = [];

    $scope.submitForm = function () {
        if ($scope.exps.length == 0) {
            $scope.$alertError("权限不能为空！");
            return;
        }

        //提交时，根据编辑模式选择调用不同的service方法
        roleService.setPermissions($scope.editRole.id, $scope.exps).then(saveOnSuccess);
    }

    function saveOnSuccess(response) {
        $scope.$alertInfo("保存成功！");

        //关闭窗口
        $scope.$close();
    }
});

mdmApp.controller("mdm.stage_index", ['$scope', 'stageService', function ($scope, stageService) {
    stageService.getStages().then(function (response) {
        $scope.stages = response.data;
    })
}]).service("stageService", function ($http, $q, dpService) {
        var services = {};
        var stagesUrl = '$/stages';

        services.getStages = function () {
            if (!services.stages) {
                return $http.get(stagesUrl).then(function (response) {
                    services.stages = response.data.filter(function (item) {
                        return [2, 3, 4].indexOf(item.id) > -1;//硬编码下只列出小初高
                    });
                    return {data: services.stages};
                });
            }
            var manualResponse = {data: services.stages};
            return $q.resolve(manualResponse);
        };

        //加上数据访问权限的学段获取函数
        services.getStagesByDP = function () {
            return services.getStages().then(function (response) {
                var stages = response.data.filter(function (s) {
                    return dpService.allowed(s.id);
                });

                return {data: stages};
            });
        }
        return services;
    })
    .filter("stageSumName", function (stageService) {
        return function (stageSum, defaultValue) {
            var tempStages = [];
            angular.forEach(stageService.stages, function (item, index) {
                if ((item.value & stageSum) == item.value) {// 位运算优先级比 == 低
                    //stage的value值与上stageSum的值如果和stage原值相等，表示有这个stage
                    tempStages.push(item.name);
                }
            });
            return (tempStages.length == 0 && defaultValue) ? defaultValue : tempStages.join(",");
        }
    });

/**
 * Created by 惠芳 on 5/2/2016.
 */
mdmApp
    .controller('mdm.subject_index', ['$log', '$scope', '$uibModal', 'subjectService', 'appConstants', function ($log, $scope, $uibModal, subjectService, appConstants) {
        subjectService.getAll().then(function (subjects) {
            $scope.subjects = subjects;
        });

        $scope.openEditSubject = function (editRole) {
            var copy = {};
            editRole && angular.copy(editRole, copy);
            $scope.subject = copy;
            $scope.editMode = editRole ? "edit" : "add";
            $uibModal.open({
                templateUrl: 'views/mdm/subject/edit.html',
                controller: 'mdm.subject_index.edit',
                scope: $scope
            }).result.then(function (rltRole) {
                if ($scope.editMode == "edit") {
                    angular.copy(rltRole, editRole);
                } else {
                    $scope.subjects.push(rltRole);
                }
            })
        };

        $scope.del = function (subject) {
            $scope.$confirm('删除确认', '您确定要删除学科【' + subject.name + '】吗?').then(function () {
                subjectService.del(subject.id).then(function () {
                    var index = $scope.subjects.indexOf(subject);
                    $scope.subjects.splice(index, 1);
                    $scope.$alertInfo("删除成功！");
                });
            })
        }

    }])
    .controller('mdm.subject_index.edit', ['$log', '$scope', 'subjectService', function ($log, $scope, subjectService) {
        //表单提交
        $scope.submitForm = function () {
            if ($scope.form.$invalid) {
                return;
            }
            var p;
            if ($scope.editMode == "add") {
                p = subjectService.add($scope.subject);
            } else {//"edit"
                p = subjectService.update($scope.subject);
            }

            //处理保存成功
            p.then(function (d) {
                $scope.$alertInfo("保存成功!");
                d && $scope.$close($scope.subject);
            });
        };
    }]);

/**
 * Created by 惠芳 on 5/3/2016.
 */

mdmApp.service('subjectService', ['$log', 'appConstants', '$http', function ($log, appConstants, $http) {
        var srv = {};
    var url = "$/subjects";
        srv.getAll = function () {
            return $http.get(url).then(function (response) {
                return response.data;
            });
        }
        srv.get = function (id) {
            return $http.get(url + '/' + id).then(function (response) {
                return response.data;
            });
        }
        srv.add = function (subject) {
            return $http.post(url, subject).then(function (response) {
                return response.data;
            });
            ;
        }
        srv.del = function (id) {
            return $http.delete(url + "/" + id).then(function (response) {
                return response.data;
            });
            ;
        }
        srv.update = function (subject) {
            return $http.put(url + "/" + subject.id, subject).then(function (response) {
                return response.data;
            });
        }
        return srv;
    }]);

/**
 * Created by 红磊 on 2016/5/11.
 */
mdmApp
    .controller('mdm.tags_index', ['$log', '$scope', '$uibModal', 'appConstants', function ($log, $scope, $uibModal, appConstants) {
        //测试数据
        $scope.tags = [{
            id: '1',
            tagName: 1,
            description: '描述信息1'
        }, {
            id: '2',
            tagName: 2,
            description: '描述信息2'
        }, {
            id: '3',
            tagName: 3,
            description: '描述信息3'
        }, {
            id: '4',
            tagName: 4,
            description: '描述信息4'
        }, {
            id: '5',
            tagName: 5,
            description: '描述信息5'
        }, {
            id: '6',
            tagName: 6,
            description: '描述信息6'
        }];
    }])

/**
 * Created by TeckyLee on 2016/6/6.
 */
mdmApp.controller("mdm.kpoint_mapping?tbId&courseId&tbName&returnUrl", function ($scope, $state, $stateParams, courseService, chapterService, $timeout, kpointService) {
    $scope.tbId = $stateParams.tbId;
    $scope.tbName = $stateParams.tbName;
    $scope.courseId = $stateParams.courseId;
    $scope.returnUrl = ($stateParams.returnUrl ? $stateParams.returnUrl : "mdm.textbook_index");
    $scope.ztreeCatolog = "ztreeCatolog";
    $scope.ztreeKPoint = "ztreeKPoint";

    courseService.getCourseById($scope.courseId).then(function (response) {
        $scope.courseName = response.data.name;
    });

    $scope.usedKPointId = [];
    function retrieveNodes() {

        //获取章节树，以及上面关联的知识点集合
        chapterService.getNodesByTextbookId($scope.tbId).then(function (response) {
            var catalogNodes = response.data;
            if (catalogNodes.length == 0) {
                $scope.$confirm("提示", "当前的教材目录是空的，请先设置教材的目录！", true).then(function () {
                    $state.go('mdm.chapter_index', {
                        tbId: $scope.tbId,
                        tbName: $scope.tbName,
                        returnUrl: $scope.returnUrl
                    });
                });
                return;
            }

            chapterService.getKPointsByTextbookId($scope.tbId).then(function (response) {
                //记录一下原始的mapping
                $scope.mappings = response.data;
                ;

                //循环每个目录节点
                angular.forEach(catalogNodes, function (node, index) {
                    //找到该节点的知识点集合
                    node.tags = getOriginalTags(node.id);

                    //展开节点
                    node.open = true;
                });

                $scope.catalogNodes = catalogNodes;
            });
        });

        //获取知识树的节点
        kpointService.getNodesByCourseId($scope.courseId).then(function (response) {
            $scope.kpointNodes = response.data;

            //展开所有的节点
            angular.forEach($scope.kpointNodes, function (node, index) {
                //因为知识树太长，所以只展开第一级即可
                if (node.parentId == 0)
                    node.open = true;
            });
        });
    };

    retrieveNodes();
    var dataSetting = {
        key: {
            name: "name"
        },
        simpleData: {
            enable: true,
            idKey: "id",
            pIdKey: "parentId"
        }
    };

    $scope.catalogSetting = {
        data: dataSetting,
        view: {
            addDiyDom: addDiyDom,
            selectedMulti: false,
            fontCss: getCatalogTreeFontCss
        },
        callback: {
            onClick: zTreeCatalogOnClick
        }
    }

    $scope.kpointSetting = {
        data: dataSetting,
        view: {
            fontCss: getKPointFontCss,
            addHoverDom: addHoverDom,
            removeHoverDom: removeHoverDom,
            selectedMulti: false
        }
    }

    function getCatalogTreeFontCss(treeId, treeNode) {
        if (treeNode.modified) {
            return {color: "#d6963b"};
        }

        return {color: "#333"};
    }

    function zTreeCatalogOnClick(event, treeId, treeNode) {
        //取消右边知识点树的选中状态
        var treeObj = $.fn.zTree.getZTreeObj($scope.ztreeKPoint);
        treeObj.cancelSelectedNode();
        setTagsBoxPosition(treeNode);
    };

    function setTagsBoxPosition(treeNode) {
        var treeNode = $("#" + treeNode.tId);
        if (treeNode.find('.tags-box').length == 0) {
            return
        }
        var winHeight = $(window).height(),
            targetTop = treeNode.offset().top,
            targetBoxHeight = treeNode.find('.dropdown-menu').height() + 22;
        if (targetTop + targetBoxHeight > winHeight) {
            treeNode.find('.timeline-panel').css('margin-top', '-' + (targetBoxHeight - 42) + 'px').addClass('timeline-panel-top');
        } else {
            treeNode.find('.timeline-panel').css('margin-top', '0').removeClass('timeline-panel-top');
        }
    }

    // $(window).resize(function(){
    //     var selectedTag=$.fn.zTree.getZTreeObj($scope.ztreeCatolog).getSelectedNodes();
    //     if(selectedTag.length==0) return;
    //     setTagsBoxPosifiton(selectedTag[0])
    // });

    //教材目录
    function getOriginalTags(catalogId) {
        var maps = $scope.mappings.filter(function (map) {
            return map.catalogId == catalogId;
        });
        var tags = [];
        angular.forEach(maps, function (map) {
            tags.push({id: map.kpointId, name: map.kpointName});
        });
        return tags;
    }

    function addDiyDom(treeId, treeNode) {
        if (!(treeNode.tags)) return;

        var target = $("#" + treeNode.tId + '_a');
        if (target.next('.tags-box').length > 0) {
            target.next('.tags-box').remove();
            target.find('.badge').remove()
        }

        if (treeNode.tags.length == 0)
            return;

        var newTagHtml = '<span class="text-success-ztree">New!</span>';
        var badgeBgColorClass = (treeNode.modified ? "label-modified" : "");
        var badge = '<span class="badge ' + badgeBgColorClass + '">' + treeNode.tags.length + '</span>';
        var tagsWrapper = $('<span class="tags-box"></span>');
        var tmp = '<div class="timeline-panel"><div class="dropdown-menu"><span class="tags-close-btn"><i class="fa fa-times-circle"></i></span><h4 class="text-center" style="font-weight:bold">知识点列表</h4><div class="divider"></div><ul>';

        //找到该节点的知识点集合
        var originalTags = getOriginalTags(treeNode.id);

        for (var i = 0; i < treeNode.tags.length; i++) {
            var tag = treeNode.tags[i];

            //判断是否是新tag
            var oldTag = originalTags.find(function (t) {
                return t.id == tag.id
            });

            var isNew = true;
            if (oldTag) isNew = false;

            tmp += '<li class="tags" kpid="' + tag.id + '" kpname="' + tag.name + '"><a href="javascript:"><span style="cursor: move;display:block;">' + tag.name + (isNew ? newTagHtml : '') + '</span><span data-role="remove"></span></a></li>';
        }
        tmp += '</ul><div class="divider"></div><ul class="clear-only"><li><a href="javascript:" class="clear-tags"><i class="fa fa-trash"></i> 清空</a></li></ul></div>';
        target.append(badge)
        target.after(tagsWrapper.html(tmp));
        tagsWrapper.on('click', '.clear-tags', function () {
            if (treeNode && treeNode.tags) {
                $scope.$confirm("清除目录上的知识点", "您确定要删除当前目录上关联的所有知识点吗？").then(function () {
                    treeNode.tags = [];
                    onNodeModified(treeNode);
                })
            }
        });
        setTagsBoxPosition(treeNode);
        tagsWrapper.on('click', '.tags-close-btn', function () {
            var treeObj = $.fn.zTree.getZTreeObj($scope.ztreeCatolog);
            treeObj.cancelSelectedNode();
        })

        //检查该元素是否已经执行过dragsort
        tagsWrapper = tagsWrapper.find('.dropdown-menu ul:first');
        if (tagsWrapper.attr('data-listidx') !== undefined) {
            tagsWrapper.dragsort('destroy')
        } else {
            tagsWrapper.dragsort({
                dragSelector: 'li.tags',
                placeHolderTemplate: "<li style='display:block;border:1px dotted #ccc;height:26px;'></li>",
                dragEnd: function () {
                    rebuildTagArray(treeNode);
                    onNodeModified(treeNode);
                }
            });
        }

        tagsWrapper.on('click', 'span[data-role="remove"]', function () {
            $(this).closest('li').remove();
            rebuildTagArray(treeNode);
            onNodeModified(treeNode);
        })
    }

    function onNodeModified(node) {
        var originalTags = getOriginalTags(node.id);

        node.modified = (JSON.stringify(node.tags) != JSON.stringify(originalTags));

        var ztree = $.fn.zTree.getZTreeObj($scope.ztreeCatolog);
        ztree.updateNode(node);
        addDiyDom($scope.ztreeCatolog, node);
    }

    function rebuildTagArray(treeNode) {
        var target = $("#" + treeNode.tId + '_a');
        var tags = [];
        var tagLi = target.next('.tags-box').find("li.tags");
        angular.forEach(tagLi, function (li) {
            tags.push({id: parseInt($(li).attr('kpid')), name: $(li).attr('kpname')});
        });
        treeNode.tags = tags;
    }

    function getTargetNode() {
        var nodes = $.fn.zTree.getZTreeObj($scope.ztreeCatolog).getSelectedNodes();
        if (nodes.length == 0) {
            return null;
        }
        return nodes[0];
    }

    //知识树
    function getKPointFontCss(treeId, treeNode) {
        if (!!treeNode.highlight) {
            return {color: "#A60000", "font-weight": "bold"};
        }
        else if (!treeNode.children || treeNode.children.length == 0) {
            return {
                color: "#168CF3",
                "font-weight": "normal"
            };
        }
        else if (treeNode.children && treeNode.children.length > 0 && (!treeNode.children[0].children || treeNode.children[0].children.length == 0)) {
            return {
                color: "#D69023",
                "font-weight": "normal"
            };
        }
        else {
            return {
                color: "#333",
                "font-weight": "normal"
            };
        }
    }

    function addHoverDom(treeId, treeNode) {
        var sObj = $("#" + treeNode.tId + "_span");
        if (treeNode.editNameFlag || $("#addBtn_" + treeNode.tId).length > 0) return;

        var isAddMode = true;
        var targetNode = getTargetNode();
        if (targetNode && targetNode.tags) {
            var existedTag = targetNode.tags.find(function (t) {
                return t.id == treeNode.id;
            });
            if (existedTag) {
                isAddMode = false;
            }
        }

        var addStr = "<span class='button add' id='addBtn_" + treeNode.tId
            + "' title='关联' onfocus='this.blur();'></span>";

        var removeStr = "<span class='button remove' id='addBtn_" + treeNode.tId
            + "' title='取消关联' onfocus='this.blur();'></span>";

        var html = isAddMode ? addStr : removeStr;

        sObj.after(html);

        var btn = $("#addBtn_" + treeNode.tId);
        if (btn) btn.bind("click", function () {
            var targetNode = getTargetNode();

            if (targetNode == null) {
                alert("请先在目录上选择一个节点！");
                return;
            }

            if (!targetNode.tags) targetNode.tags = [];
            var existedTag = targetNode.tags.find(function (t) {
                return t.id == treeNode.id;
            });
            if (existedTag) {
                var index = targetNode.tags.indexOf(existedTag);
                targetNode.tags.splice(index, 1);
            }
            else {
                targetNode.tags.push({id: parseInt(treeNode.id), name: treeNode.name});
            }

            onNodeModified(targetNode);
            removeHoverDom(treeId, treeNode);
            return false;
        });
    }

    function removeHoverDom(treeId, treeNode) {
        $("#addBtn_" + treeNode.tId).unbind().remove();
    }

    //知识树搜索
    $scope.nodeList = [];
    $scope._query = '';
    $scope.search = function (keyEvent) {
        $scope.query = $scope._query.trim();
        var hasValidQuery = ($scope.query && $scope.query.length > 0);
        var treeObj = $.fn.zTree.getZTreeObj($scope.ztreeKPoint);
        updateNodes(treeObj, false);
        treeObj.hideNodes(treeObj.getNodes());

        if (hasValidQuery) {
            $scope.nodeList = treeObj.getNodesByParamFuzzy("name", $scope.query, null);

            //显示匹配的节点
            var shownNodesList = [];
            for (var i = 0; i < $scope.nodeList.length; i++) {
                var node = $scope.nodeList[i];
                shownNodesList = shownNodesList.concat(node.getPath());
            }

            if (shownNodesList.length < 100) {
                for (var i = 0; i < shownNodesList.length; i++) {
                    treeObj.expandNode(shownNodesList[i], true, false);
                }
            }

            treeObj.showNodes(shownNodesList);
            updateNodes(treeObj, true);

            //把多显示的节点去掉
            var realShownNodes = treeObj.getNodesByParam("isHidden", false);
            for (var i = 0; i < realShownNodes.length; i++) {
                var node = realShownNodes[i];
                if (shownNodesList.indexOf(node) < 0) {
                    treeObj.hideNode(node);
                }
            }
        }
        else {
            //显示所有隐藏的节点
            var allHiddenNodes = treeObj.getNodesByParam("isHidden", true);
            treeObj.showNodes(allHiddenNodes);
        }
    }

    function updateNodes(zTree, highlight) {

        var len = $scope.nodeList.length;
        for (var i = 0; i < len; i++) {
            $scope.nodeList[i].highlight = highlight;
            zTree.updateNode($scope.nodeList[i]);
        }
    }

    //save & resume action
    $scope.resume = function () {
        //设置catalogNodes，会出发ztree重新初始化数据
        $scope.catalogNodes = angular.copy($scope.catalogNodes);
    };

    $scope.goBack = function () {
        $scope.$confirm("保存提醒", "页面右下角有[保存]按钮哦，不忘要忘记保存您的操作！您确定要离开吗？").then(function () {
            $scope.$goByUiSref($scope.returnUrl);
        });
    };

    $scope.saveChanges = function () {
        //collect all tags
        var treeObj = $.fn.zTree.getZTreeObj($scope.ztreeCatolog);
        var nodes = treeObj.transformToArray(treeObj.getNodes());

        var mappings = [];
        angular.forEach(nodes, function (node, key) {
            if (node.tags) {
                angular.forEach(node.tags, function (tag, index) {
                    mappings.push({
                        catalogId: node.id,
                        kpointId: tag.id,
                        kpointName: tag.name,
                        ordinal: index
                    });
                })
            }
        });

        chapterService.saveKPointChanges($scope.tbId, mappings).then(function (response) {
            $scope.$alertInfo("保存成功！");
        });
    }
    $scope.export = function () {
        var zTree = $.fn.zTree.getZTreeObj($scope.ztreeCatolog);
        var nodes = zTree.transformToArray(zTree.getNodes());
        var nodeNames = [];
        angular.forEach(nodes, function (node, index) {
            var row = [];
            row[parseInt(node.level)] = node.name;
            if (node.tags && node.tags.length > 0) {
                var tags = '';
                for (var idx in node.tags) {
                    tags += (parseInt(idx) ? '|' : ':') + node.tags[idx].name;
                }

                row[row.length] = tags;
            }

            nodeNames.push(row);
        });
        csvExporter().exportJson(nodeNames, false, $scope.tbName + ' - 目录及知识点');
    }
});

/**
 * Created by TeckyLee on 2016/5/27.
 */
mdmApp.controller('mdm.textbook_index.edit', function ($scope, stageService, gradeService, textbookService, courseService, param) {
    $scope.editMode = param.editMode;
    $scope.editBook = param.editBook;
    if (!$scope.editBook.term) {
        $scope.editBook.term = "";
    }

    courseService.getCourses().then(function (response) {
        $scope.courses = response.data;
        if ($scope.editMode == 'edit') {
            var course = $scope.courses.find(function (c) {
                return c.id == $scope.editBook.courseId;
            });

            //根据课程，设置学段字段
            if (course != null) {
                $scope.stageId = course.stageId;
            }
        }
        else {
            //默认选中高中，因为高中的使用频率可能比较高
            $scope.stageId = 4;
        }

        $scope.refreshGradesByStageId();
        $scope.onCourseChanged();
    });

    $scope.grades = [];
    stageService.getStages().then(function (response) {
        $scope.stages = response.data;

    });

    //学段变化，刷新年级
    $scope.onStageChanged = function () {
        $scope.refreshGradesByStageId();
    };

    //课程变化，刷新教材版本下拉框
    $scope.onCourseChanged = function () {
        if (!$scope.editBook.courseId || $scope.editBook.courseId == "") {
            $scope.versions = [];
        }
        else {
            courseService.getVersionsByCourseId($scope.editBook.courseId).then(function (response) {
                $scope.versions = response.data;
            })
        }
    };

    $scope.refreshGradesByStageId = function () {
        gradeService.getGradesByStageId($scope.stageId).then(function (response) {
            $scope.grades = response.data;
        })
    };

    //只有小初，才需要设置年级。高中的教材，年级非必填
    $scope.checkGradeRequired = function () {
        return $scope.stageId == 2 || $scope.stageId == 3;
    };

    $scope.submitForm = function () {
        if (!$scope.editForm.$valid) {
            return;
        }

        if ($scope.editBook.term == "") {
            //服务器反序列化枚举的时候，如果term为空字符串，反序列化会失败。
            // 在客户端删除掉term属性，服务器反序列化会将term设置为null
            delete $scope.editBook.term;
        }

        if ($scope.editMode == 'add') {
            textbookService.add($scope.editBook).then(saveOnSuccess);
        }
        else {
            textbookService.update($scope.editBook).then(saveOnSuccess);
        }
    };

    function saveOnSuccess(response) {
        $scope.$alertInfo("保存成功！");

        //新增的时候，返回的是book对象；更新操作，返回的bool值.所以获取id的方式不一样
        var id = ($scope.editMode == 'add' ? response.data.id : $scope.editBook.id);

        //从服务器刷新TextbookEx数据，因为有name字段需要翻译
        textbookService.getBookById(id).then(function (newResponse) {

            //关闭窗口，并设置modal的result
            $scope.$close(newResponse.data);

        }, function () {
            $scope.$alertError("刷新数据失败，请手动刷新列表！");

            var book = ($scope.editMode == 'add' ? response.data : $scope.editBook);
            //关闭窗口，并设置modal的result
            $scope.$close(book);
        });
    }

    $scope.autoCompleteName = function () {

        var name = "";

        var courseName = "";
        if ($scope.editBook.courseId) {
            var course = $scope.courses.find(function (c) {
                return c.id == $scope.editBook.courseId;
            });
            course && (courseName = course.name);
        }

        var versionName = "";
        if ($scope.editBook.versionId) {
            var version = $scope.versions.find(function (v) {
                return v.id == $scope.editBook.versionId;
            });
            version && (versionName = version.name);
        }

        var volume = $scope.editBook.volume;

        var gradeName = "";
        if ($scope.editBook.gradeId) {
            var grade = $scope.grades.find(function (g) {
                return g.id == $scope.editBook.gradeId;
            });
            grade && (gradeName = grade.name);
        }

        var termName = "";
        if ($scope.editBook.term)
            termName = textbookService.getTermName($scope.editBook.term)

        name = courseName + versionName + (volume ? volume : (gradeName + termName));

        $scope.editBook.name = name;
    }
});

/**
 * Created by TeckyLee on 2016/5/26.
 */
mdmApp.controller("mdm.textbook_index?stageId&courseId&versionId&keywords&pageIndex", function ($scope, $state, $stateParams, stageService, gradeService, textbookService, courseService, appConstants, $uibModal) {
    $scope.totalSize = 0;
    $scope.params = {pageSize: appConstants.pageSize, currentPage: 1};
    if ($stateParams.pageIndex) {
        $scope.params.currentPage = parseInt($stateParams.pageIndex);
    }

    $scope.books = [];
    $scope.params.nameKeyword = $stateParams.keywords;
    $scope.grades = [];
    stageService.getStagesByDP().then(function (response) {
        $scope.stages = response.data;

        //默认选中倒数第一个
        $scope.params.stageId = ($stateParams.stageId ? parseInt($stateParams.stageId) : $scope.stages[$scope.stages.length - 1].id);
        $scope.onStageChanged();
    });

    $scope.onStageChanged = function () {
        refreshCoursesByStageId();
        //$scope.refreshGradesByStageId();
    };

    function refreshCoursesByStageId() {
        courseService.getCoursesByDP($scope.params.stageId).then(function (response) {
            $scope.courses = response.data;

            if ($stateParams.courseId) {
                $scope.params.courseId = parseInt($stateParams.courseId);
                $stateParams.courseId = null;
            }
            else {
                $scope.params.courseId = $scope.courses[0].id;
            }
            $scope.onCourseChanged();
        });
    }

    $scope.onCourseChanged = function () {

        courseService.getVersionsByCourseId($scope.params.courseId).then(function (response) {
            $scope.versions = response.data;

            if ($stateParams.versionId) {
                $scope.params.versionId = parseInt($stateParams.versionId);
                $stateParams.versionId = null;
            }

            $scope.onVersionChanged();
        });
    };

    $scope.onVersionChanged = function () {
        $scope.retrieveBooks();
    };

    //$scope.refreshGradesByStageId = function () {
    //    gradeService.getGradesByStageId($scope.params.stageId).then(function (response) {
    //        $scope.grades = response.data;
    //    })
    //};

    $scope.getReturnUrl = function () {
        var params = {};
        params.stageId = $scope.params.stageId;
        params.courseId = $scope.params.courseId;
        params.versionId = $scope.params.versionId;
        params.keywords = $scope.params.nameKeyword;
        params.pageIndex = $scope.params.currentPage;

        return "mdm.textbook_index(" + JSON.stringify(params) + ")";
    }

    $scope.retrieveBooks = function (keyEvent) {
        if (keyEvent && keyEvent.which != 13) {
            return;
        }
        textbookService.getAllBooks($scope.params).then(function (response) {
            var pagination = response.data;
            $scope.books = pagination.items;
            $scope.params.currentPage = pagination.currentPage;
            $scope.params.pageSize = pagination.pageSize;
            $scope.totalSize = pagination.totalSize;
            $scope.totalPage = pagination.totalPage;
        });
    };

    //页码切换
    $scope.pageChanged = function (newPageNum) {
        $scope.params.currentPage = newPageNum;
        $scope.retrieveBooks();
    };

    $scope.deleteBook = function (book) {
        var index = $scope.books.indexOf(book);
        $scope.$confirm('删除教材', '您确定要删除教材[' + book.name + ']及其目录吗？').then(function (modalResult) {
            if (!modalResult) return;

            //执行删除
            textbookService.delete(book.id).then(function (response) {
                if (response.data) {
                    $scope.$alertInfo("删除成功！");
                    //从当前列表中删除指定项
                    $scope.books.splice(index, 1);
                }
                else {
                    $scope.$alertError("删除失败！");
                }
            });
        });
    };

    $scope.editMode = 'add';
    $scope.editBook = {};
    var modalOptions = {
        templateUrl: 'views/mdm/textbook/edit.html',
        controller: 'mdm.textbook_index.edit',
        backdrop: 'static',
        resolve: {
            param: function () {
                return {
                    editMode: $scope.editMode,
                    editBook: $scope.editBook
                };
            }
        }
    };

    //打开新增模态框
    $scope.openAddWindow = function () {
        $scope.editBook = {};
        $scope.editMode = 'add';
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            $scope.books.unshift(result);
            $scope.totalSize++;
        });
    };

    //打开更新窗口
    $scope.openEditWindow = function (book) {
        $scope.editMode = 'edit';
        $scope.editBook = angular.copy(book);
        var modalInstance = $uibModal.open(modalOptions).result.then(function (result) {
            angular.copy(result, book);
        });
    };

    $scope.getTermName = function (term) {
        return textbookService.getTermName(term);
    }

}).service('textbookService', function ($http) {
    var service = {};
    var baseUrl = "$/textbooks";
    service.getAllBooks = function (params) {
        return $http.get(baseUrl, {"params": params});
    }

    service.delete = function (id) {
        return $http.delete(baseUrl + "/" + id);
    }

    service.add = function (book) {
        return $http.post(baseUrl, book);
    }

    service.update = function (book) {
        return $http.put(baseUrl + "/" + book.id, book)
    }

    service.getBookById = function (id) {
        return $http.get(baseUrl + "/" + id)
    }

    service.getTermName = function (term) {
        if (!term) return "";
        var chTerm = "";
        switch (term.toUpperCase()) {
            case  "LAST":
                chTerm = "上册";
                break;
            case "NEXT":
                chTerm = "下册";
                break;
            default:
                chTerm = " ";
                break;

        }
        return chTerm;
    }
    return service;
});

/**
 * Created by 惠芳 on 5/25/2016.
 */
mdmApp
    .controller('mdm.textbookversion_index', ['$scope', '$uibModal', 'textbookVersionService', 'stageService', 'orgService', function ($scope, $uibModal, textbookVersionService, stageService, orgService) {
        $scope.params = {key: "", stageValue: 0};

        stageService.getStages().then(function (response) {
            $scope.stages = response.data;
        }).then(function () {
            textbookVersionService.getTextbookVersions().then(function (response) {
                $scope.allVersions = response.data;
                $scope.versions = response.data;
            });
        });

        $scope.filterVersion = function (version) {
            var key = $scope.params.key.trim();
            var flag = true;
            if (key) {
                flag = version.name.indexOf(key) > -1 || version.id.toString().indexOf(key) > -1;
            }
            if ($scope.params.stageValue > 0) {
                flag = flag && ((version.stageSum & $scope.params.stageValue) == $scope.params.stageValue);
            }
            if ($scope.params.stageValue == -1) {//-1代表五四制
                flag = flag && version.gradeDivision == "G54";
            }
            return flag;
        };

        $scope.openEditVersion = function (editVersion) {
            var copy = {};
            editVersion && angular.copy(editVersion, copy);
            var editMode = editVersion ? "edit" : "add";
            var stagesCopy = angular.copy($scope.stages);
            if (editVersion) {
                angular.forEach(stagesCopy, function (item, index) {
                    if ((item.value & editVersion.stageSum) == item.value) {
                        item.checked = true;
                    }
                });
            }
            $uibModal.open({
                templateUrl: 'views/mdm/textbookversion/edit.html',
                controller: 'mdm.textbookversion_index.edit',
                resolve: {
                    params: function () {
                        return {
                            editMode: editMode,
                            version: copy,
                            stages: stagesCopy
                        };
                    }
                },
                backdrop: "static"
            }).result.then(function (rltVersion) {
                if (editMode == "edit") {
                    angular.copy(rltVersion, editVersion);
                } else {
                    $scope.versions.push(rltVersion);
                }
            })
        };

        $scope.delVersion = function (version) {
            $scope.$confirm('删除确认', '您确定要删除教材版本【' + version.name + '】吗? 注意此操作不可恢复！').then(function () {
                textbookVersionService.deleteTextbookVersion(version.id).then(function () {
                    var index = $scope.versions.indexOf(version);
                    $scope.versions.splice(index, 1);
                    $scope.$alertInfo("删除成功！");
                });
            })
        }
    }])
    .controller('mdm.textbookversion_index.edit', function ($log, $scope, textbookVersionService, params) {
        $scope.editMode = params.editMode;
        $scope.version = params.version;
        $scope.stages = params.stages;
        $scope.hasCheckedStage = function () {
            return $scope.stages.some(function (item) {
                return item.checked;
            })
        }
        //表单提交
        $scope.submitForm = function () {
            if ($scope.form.$invalid) {
                return;
            }
            //计算勾选的stageSum
            $scope.version.stageSum = $scope.stages.reduce(function (prev, next) {
                return prev + (next.checked ? next.value : 0)
            }, 0);
            $scope.version.gradeDivision = $scope.version.isG54 ? "G54" : "G63";
            var p;
            if ($scope.editMode == "add") {
                p = textbookVersionService.addTextbookVersion($scope.version);
            } else {//"edit"
                p = textbookVersionService.updateTextbookVersion($scope.version);
            }

            //处理保存成功
            p.then(function (response) {
                $scope.$alertInfo("保存成功!");
                response && $scope.$close($scope.editMode == "add" ? response.data : $scope.version);
            });
        };
    })
    .service('textbookVersionService', ['$http', function ($http) {
        var textbookVersionUrl = '$/textbook/versions';
        var courseUrlTmpl = '$/courses/{courseId}/textbookversions';
        return {
            /**
             * 获取教材版本列表
             * @param stageId 不传则获取所有
             * @returns {*}
             */
            getTextbookVersions: function (stageId) {
                return $http.get(textbookVersionUrl, {params: {'stageid': stageId || ""}});
            },
            /**
             * 获取指定课程关联的所有教材版本
             * @param courseId
             * @returns {*}
             */
            getTextbookVersionsByCourseId: function (courseId) {
                return $http.get(courseUrlTmpl.replace("{courseId}", courseId));
            },
            updateTextbookVersion: function (version) {
                return $http.put(textbookVersionUrl + "/" + version.id, version);
            },
            addTextbookVersion: function (version) {
                return $http.post(textbookVersionUrl, version);
            },
            deleteTextbookVersion: function (id) {
                return $http.delete(textbookVersionUrl + "/" + id);
            },
        }
    }]);

/**
 * Created by TeckyLee on 2016/6/2.
 */
mdmApp.service('dpService', function (subject) {

    var services = {};
    services.allowed = function (inputExp) {
        //inputExp是否允许访问，有一下几种情况(假定inputExp是2:31)：
        //1. 当前用户的权限是mdm.data或者是*
        //2. 用户拥有2的权限，那么自然拥有2：31的权限
        //3. 用户直接拥有2：31的权限
        //4. 用户拥有2：31：6的权限，那么2：31肯定也得允许
        inputExp = "mdm.data:" + inputExp + ":r";
        return subject.isPermitted(inputExp);
    }
    return services;
});

/**
 * Created by TeckyLee on 2016/5/19.
 */
mdmApp.controller("mdm.user_index.assign_roles", function ($scope, applicationService, roleService, userService, param) {
    $scope.activeTabIndex = 1;
    $scope.selectedApp = null;
    $scope.user = param;

    //region application pane
    applicationService.getApplications().then(function (response) {
        $scope.applicationList = response.data;
        $scope.selectedApp = response.data[0];
    });

    $scope.selectApp = function (app) {
        $scope.selectedApp = app;
    }
    //endregion

    //region prev & next
    $scope.prev = function () {
        if ($scope.activeTabIndex > 1)
            $scope.activeTabIndex--;
    }

    $scope.next = function () {
        if ($scope.activeTabIndex < 4)
            $scope.activeTabIndex++;
    }
    //endregion

    //region role pane related
    $scope.stepOutMenuRolePane = function () {

        var checkedRoles = $scope.menuRoles.filter(function (role) {
            return role.checked;
        }).map(function (role) {
            return role.id
        });

        if (checkedRoles.length == 0) {
            $scope.$alertError("至少要为用户选择一个角色！");
            return;
        }

        //保存用户选择的角色
        userService.resetRolesByUserId(param.userId, 'MENU', checkedRoles, $scope.selectedApp.id).then(function () {
            $scope.$alertInfo("角色设置成功！");
            //进入数据权限设置
            stepIntoDataRolePane();
        });
    }

    $scope.stepIntoMenuRolePane = function () {
        roleService.getRoles($scope.selectedApp.id, 'MENU').then(function (response) {
            $scope.menuRoles = response.data;
            if ($scope.menuRoles == null || $scope.menuRoles.length == 0) {
                $scope.$alertError("该应用还没有定义角色，不能进行下一步！");
                return;
            }
            //获取现在用户已经有的roles，并设置选中状态
            getCheckedMenuRoles();
            $scope.next();
        });
    }

    function getCheckedMenuRoles() {
        userService.getRolesByUserId(param.userId, 'MENU', $scope.selectedApp.id).then(function (response) {
            var ownedRoleIds = response.data;

            angular.forEach($scope.menuRoles, function (value, key) {
                value.checked = (ownedRoleIds.indexOf(value.id) >= 0);
            });
        });
    }

    $scope.selectRole = function (role) {
        role.checked = !role.checked;
    }
    //endregion

    //region 数据访问权限
    function stepIntoDataRolePane() {
        roleService.getRoles($scope.selectedApp.id, 'DATA').then(function (response) {
            $scope.dataRoles = response.data;
            if ($scope.dataRoles == null || $scope.dataRoles.length == 0) {
                $scope.$alertError("该应用还没有定义数据权限，不能进行下一步！");
                return;
            }
            //获取现在用户已经有的数据权限roles，并设置选中状态
            getCheckedDataRoles();
            $scope.next();
        });
    }

    function getCheckedDataRoles() {
        userService.getRolesByUserId(param.userId, 'DATA', $scope.selectedApp.id).then(function (response) {
            var ownedRoleIds = response.data;

            angular.forEach($scope.dataRoles, function (value, key) {
                value.checked = (ownedRoleIds.indexOf(value.id) >= 0);
            });
        });
    }

    $scope.stepOutDataRolePane = function () {

        var checkedRoles = $scope.dataRoles.filter(function (role) {
            return role.checked;
        }).map(function (role) {
            return role.id;
        });

        if (checkedRoles.length == 0) {
            $scope.$alertError("至少要为用户选择一个数据权限！");
            return;
        }

        //保存用户选择的角色
        userService.resetRolesByUserId(param.userId, 'DATA', checkedRoles, $scope.selectedApp.id).then(function () {
            $scope.$alertInfo("数据权限设置成功！");
            $scope.next();
        });
    }

    //endregion

})

/**
 * Created by 惠芳 on 5/2/2016.
 */
mdmApp
    .controller('mdm.user_index?appId', function ($log, $stateParams, $scope, $uibModal, appConstants, applicationService, userService) {
        $scope.params = {};
        $scope.params.currentPage = 1;
        $scope.params.pageSize = appConstants.pageSize;
        $scope.totalSize = 0;
        $scope.totalPage = 0;

        if ($stateParams.appId) {
            $scope.params.appId = $stateParams.appId;
        }

        //获取所有的应用
        applicationService.getApplications().then(function (response) {
            $scope.apps = response.data;
        });

        //分页获取用户列表方法
        $scope.listUsers = function (keyEvent) {
            if (keyEvent && keyEvent.which != 13) {
                return;
            }
            if ($scope.params.username) {
                $scope.params.username = $scope.params.username.trim();
            }
            userService.getUsers($scope.params).then(function (response) {
                $scope.users = response.data.items;
                $scope.params.currentPage = response.data.currentPage;
                $scope.params.pageSize = response.data.pageSize;
                $scope.totalSize = response.data.totalSize;
                $scope.totalPage = response.data.totalPage;
            });
        }

        $scope.listUsers();

        //页码切换
        $scope.pageChanged = function (pageNo) {
            $scope.currentPage = pageNo;
            $scope.listUsers();
        }

        $scope.openEditUser = function (editUser) {
            var copy = {};
            editUser && angular.copy(editUser, copy);
            $uibModal.open({
                templateUrl: 'views/mdm/user/edit.html',
                controller: 'mdm.user_index.edit',
                resolve: {
                    param: function () {
                        return {'user': copy};
                    }
                }
            }).result.then(function (rltUser) {
                angular.copy(rltUser, editUser);
            })
        };

        $scope.openAddUser = function () {
            $uibModal.open({
                templateUrl: 'views/mdm/user/reg_user.html',
                controller: 'mdm.user_index.reg_user',
                backdrop: "static"
            }).result.then(function (rltUser) {
                $scope.users.splice(0, 0, rltUser);//新增的插入到列表第一行，这样醒目一点
                $scope.totalSize = $scope.totalSize + 1;

                //提示用户是否要继续设置权限
                $scope.$confirm("继续为" + rltUser.username + "授权",
                    "接下来您需要设置用户的角色和数据访问权限。 如果您现在没有时间，也可以稍后在列表中点击 [授权] 按钮来完成！").then(function () {
                    $scope.openModalForAssignRoles(rltUser);
                }, function () {
                    $scope.$alertWarning("提醒：不要忘了给用户设置合适的角色和权限！");
                });
            })
        };
        $scope.openModalForAssignRoles = function (user) {
            $uibModal.open({
                templateUrl: 'views/mdm/user/assign_roles.html',
                controller: 'mdm.user_index.assign_roles',
                backdrop: "static",
                resolve: {
                    param: function () {
                        return {'userId': user.id, 'userName': user.username};
                    }
                }
            })
        }
        $scope.del = function (user) {
            $scope.$confirm('删除确认', '您确定要删除用户【' + user.username + '】吗?').then(function () {
                userService.deleteUser(user.id).then(function () {
                    var index = $scope.users.indexOf(user);
                    $scope.users.splice(index, 1);
                    $scope.$alertInfo("删除成功！");
                    $scope.totalSize = $scope.totalSize - 1;
                });
            })
        }
    })
    .controller('mdm.user_index.edit', function ($scope, userService, param) {
        //该controller被header notification指令复用，用来修改当前登录用户的信息
        $scope.user = param.user;

        //表单提交
        $scope.submitForm = function () {
            if ($scope.form.$invalid) {
                return;
            }

            userService.updateUser($scope.user).then(function (res) {
                //处理保存成功
                $scope.$alertInfo("保存成功!");
                res && $scope.$close(res.data);
            });
        };
    })
    .service("userService", ["$http", function ($http) {
        var userSrv = {};
        var userUrl = "$/users";
        var xkwUserUrl = "$/xkwusers";

        userSrv.getUsers = function (params) {
            return $http.get(userUrl, {"params": params});
        }

        userSrv.getXkwUser = function (username) {
            return $http.get(xkwUserUrl + "/" + username);
        }

        userSrv.addUser = function (user) {
            return $http.post(userUrl, user);
        }

        userSrv.updateUser = function (user) {
            return $http.put(userUrl + "/" + user.id, user);
        }

        userSrv.deleteUser = function (id) {
            return $http.delete(userUrl + "/" + id);
        }

        userSrv.getUserByName = function (username) {
            return $http.get(userUrl + "/" + username);
        }

        userSrv.getUserByNameForReg = function (username) {
            return $http.get(userUrl + "/regcheck/" + username);
        }

        userSrv.getRolesByUserId = function (userId, roleType, appId) {
            return $http.get(userUrl + "/" + userId + "/" + appId + "/" + roleType + "/roles");
        }

        userSrv.resetRolesByUserId = function (userId, roleType, roleIds, appId) {
            return $http.put(userUrl + "/" + userId + "/" + appId + "/" + roleType + "/roles", roleIds);
        }

        userSrv.getCurrentUser = function () {
            return $http.get("$/loginuser");
        }
        return userSrv;
    }]);

/**
 * Created by TeckyLee on 2016/5/18.
 */
mdmApp.controller("mdm.user_index.reg_user", function ($scope, userService) {
    $scope.userName = "";
    $scope.validUserName = false;
    $scope.user = null;
    $scope.queryUser = function () {
        $scope.user = null;
        $scope.validUserName = false;

        if ($scope.queryForm.$invalid) {
            return;
        }
        userService.getUserByNameForReg($scope.userName).then(function (response) {
            $scope.user = response.data;
            $scope.validUserName = true;
        });
    }
    $scope.isRegistering = false;
    $scope.regUser = function () {

        if ($scope.editForm.$invalid) {
            $scope.editForm.$submitted = true;
            return;
        }
        $scope.isRegistering = true;
        userService.addUser($scope.user).then(function (res) {
            $scope.$alertInfo("注册成功!");
            var user = res.data;
            $scope.$close(user);
        }, function () {
            //失败的时候
            $scope.isRegistering = false;
        });
    }

    $scope.changeUser = function () {
        $scope.user = null;
        $scope.validUserName = false;
    }
});

/**
 * Created by 红磊 on 2016/5/9.
 *
 *
 *example: <div ng-init="areas='340203';level='province'">
 <div class="well">
 当前选中区域代码:{{areas}}||{{level}}
 </div>
 <div area-selector area-id="areas" area-level="level" width="400"></div>
 </div>
 *
 */

qbmApp.directive('areaSelector', function (mdService) {
    return {
        templateUrl: 'directives/areaselector/areaselector.html',
        restrict: 'AE',
        replace: true,
        scope: {
            areaId: '=',
            areaLevel: '=',
            width: '@'
        },
        compile: function () {
            return {
                pre: function (scope, element, attrs) {
                    element.on("click", "[data-stopPropagation]", function (e) {
                        e.stopPropagation();
                    }).css('width', (scope.width || 200) + 'px');
                    var wrapper = element.find('.input-group-btn:first');
                    var dropdown = element.find('.dropdown-menu:first');
                    var wrapperPositionLeft = element.offset().left;
                    if ($(window).width() - wrapperPositionLeft < 500) {
                        dropdown.css({right: 0, left: 'auto'});
                    }
                    scope.closePanel = function () {
                        wrapper.removeClass('open');
                    };
                    scope.openPanel = function () {
                        wrapper.addClass('open');
                        wrapper.attr('aria-haspopup', 'true');
                        wrapper.attr('aria-expanded', 'true')
                    };
                    element.find('input.area-selector-input').on('focus click', function () {
                        scope.openPanel();
                        return false;
                    })
                },
                post: function (scope, element, attrs) {
                    mdService.getAreas().then(function (response) {
                        scope.allData = response.data;
                        scope.provinces = scope.getProvinces();
                        if (scope.areaId) {
                            var isProv = !!parseInt((scope.areaId + '').substring(0, 2));
                            var isCity = !!parseInt((scope.areaId + '').substring(2, 4));
                            var isCounty = !!parseInt((scope.areaId + '').substring(4, 6));

                            var county, city, province;
                            if (isCounty) {
                                county = scope.getAreaObject(scope.areaId);
                                city = scope.getAreaObject(county.parentId);
                                province = scope.getAreaObject(city.parentId);
                            } else if (isCity) {
                                city = scope.getAreaObject(scope.areaId);
                                province = scope.getAreaObject(city.parentId);
                            } else if (isProv) {
                                province = scope.getAreaObject(scope.areaId);
                            }
                            //必须按照省市县的层级一步一步选择
                            if (isProv) {
                                scope.selectProvince(province);
                                if (isCity) {
                                    scope.selectCity(city);
                                    if (isCounty) {
                                        scope.selectCounty(county);
                                    }
                                }
                            }
                        }
                    });
                }
            }
        },
        controller: function ($scope) {
            $scope.expanded = false;
            $scope.cities = [];
            $scope.counties = [];
            $scope.flag = {
                currentTag: 'province',
                selectedProvince: null,
                selectedCity: null,
                selectedCounty: null
            };
            $scope.result = {
                selectedName: ''
            };

            //console.log($scope)
            // $scope.$apply(function($scope){
            //     $scope.opt.id='10000'
            // })

            $scope.getProvinces = function () {
                var provinces = [];
                angular.forEach($scope.allData, function (value, key) {
                    if (value.level == "PROVINCE") {
                        provinces.push(value);
                    }
                });
                return provinces;
            };
            $scope.getAreaData = function (parentId) {
                var data = [];
                for (var index in $scope.allData) {
                    if ($scope.allData[index].parentId == parentId) {
                        data.push($scope.allData[index]);
                    }
                }
                return data;
            };
            $scope.getAreaObject = function (id) {
                var obj = null;
                for (var value in $scope.allData) {
                    if ($scope.allData[value].id == id) {
                        obj = $scope.allData[value];
                        break;
                    }
                }
                return obj;
            };
            $scope.selectProvince = function (argument) {
                $scope.flag.selectedCity = null;
                $scope.counties = [];
                $scope.cities = $scope.getAreaData(argument.id);
                $scope.flag.selectedProvince = argument;
                if ($scope.cities.length == 1) {
                    $scope.selectCity($scope.cities[0]);
                } else {
                    if (argument.id < 10) {
                        $scope.closePanel();
                    } else {
                        $scope.flag.currentTag = 'city';
                        $scope.areaLevel = 'province';
                    }
                };
                $scope.areaId = argument.id

            };
            $scope.selectCity = function (argument) {
                $scope.flag.selectedCounty = null;
                $scope.counties = $scope.getAreaData(argument.id);
                $scope.flag.selectedCity = argument;
                $scope.flag.currentTag = 'county';
                $scope.areaLevel = 'city';
                $scope.areaId = argument.id
            }
            $scope.selectCounty = function (argument) {
                $scope.flag.selectedCounty = argument;
                $scope.areaLevel = 'county';
                $scope.areaId = argument.id
                $scope.closePanel();
            }
            $scope.tabTo = function (value) {
                (parseInt(value) !== $scope.flag.currentTag) && ($scope.flag.currentTag = value);
            };
            $scope.areaClear = function () {
                $scope.cities = [];
                $scope.counties = [];
                $scope.areaLevel = '';
                $scope.areaId = '';
                $scope.result.selectedName = '';
                $scope.flag.currentTag = "province";
                $scope.flag.selectedProvince = null;
                $scope.closePanel();
            }
            $scope.$watch("flag", function () {
                if ($scope.flag.selectedProvince) {
                    $scope.result.selectedName = $scope.flag.selectedProvince.name;
                    if ($scope.flag.selectedCity) {
                        if ($scope.flag.selectedCity.shortName !== $scope.flag.selectedProvince.shortName) {
                            $scope.result.selectedName += " / " + $scope.flag.selectedCity.name;
                        } else {
                            $scope.result.selectedName = $scope.flag.selectedCity.name;
                        }
                        if ($scope.flag.selectedCounty) {
                            $scope.result.selectedName += " / " + $scope.flag.selectedCounty.name;
                        }
                        ;
                    }
                }
            }, true)
        }
    }
})

'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('chat', function () {
        return {
            templateUrl: 'directives/chat/chat.html',
            restrict: 'E',
            replace: true,
        }
    });



'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('stats', function () {
        return {
            templateUrl: 'directives/dashboard/stats/stats.html',
            restrict: 'E',
            replace: true,
            scope: {
                'model': '=',
                'comments': '@',
                'number': '@',
                'name': '@',
                'colour': '@',
                'details': '@',
                'type': '@',
                'goto': '@'
            }

        }
    });

'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('headerNotification', function () {
        return {
            templateUrl: 'directives/header/header-notification/header-notification.html',
            restrict: 'E',
            replace: true,
            controller: function ($scope, $uibModal, userService) {
                $scope.openEditModal = function () {
                    if (!$scope.loginUser) return;

                    var copy = {};
                    angular.copy($scope.loginUser, copy);

                    $uibModal.open({
                        templateUrl: 'views/mdm/user/edit.html',
                        controller: 'mdm.user_index.edit',
                        resolve: {
                            param: function () {
                                return {'user': copy};
                            }
                        }
                    }).result.then(function (rltUser) {
                        angular.copy(rltUser, $scope.loginUser);
                    });
                }

                userService.getCurrentUser().then(function (response) {
                    $scope.loginUser = response.data;
                });
            }

        }
    });



'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('header', function () {
        return {
            templateUrl: 'directives/header/header.html',
            restrict: 'E',
            replace: true,
            link: function (scope, element) {
                element.find('.menu-control').on('click', function () {
                    $('body').toggleClass('mini-navbar')
                })
            }
        }
    });



'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('notifications', function () {
        return {
            templateUrl: 'directives/notifications/notifications.html',
            restrict: 'E',
            replace: true,
        }
    });



angular.module('mdmApp').directive('pageList', function () {
    return {
        scope: {
            totalSize: '='
        },
        template: '<div class="text-muted mart10 pull-left">共 {{totalSize}} 条记录</div>',
        replace: true,
        restrict: 'EA'
    }
})

/**
 * Created by TeckyLee on 2016/5/17.
 */
mdmApp.directive("permissionBuilder", function ($timeout) {
        return {
            restrict: 'A',
            templateUrl: 'directives/permission/permissionbuilder.html',
            replace: false,
            scope: {
                shiroExps: "=",
                permissionTree: "="
            },
            controller: function ($scope) {
                $scope._isInitialized = false;
                $scope.ztreeId = Math.random().toString().replace("0.", "ztree");
                $scope.settings = {
                    data: {
                        simpleData: {
                            enable: true,
                            idKey: "id",
                            pIdKey: "parentId"
                        }
                    },
                    check: {
                        enable: true
                    },
                    callback: {
                        onCheck: onCheckHandler
                    }
                };

                $scope.onTreeInitialized = function () {
                    checkNodesByShiroExps();
                }

                function onCheckHandler(event, treeId, treeNode) {
                    //change permissionTree
                    var tempShiroExps = [];
                    var ztree = $.fn.zTree.getZTreeObj(treeId);
                    var checkedNodes = ztree.getCheckedNodes(true);
                    for (var idx in checkedNodes) {
                        var node = checkedNodes[idx];

                        if (!node.getCheckStatus().half) {
                            var parentNode = node.getParentNode();
                            if (parentNode == null || parentNode.getCheckStatus().half) {
                                buildShiroExp(node, tempShiroExps);
                            }
                        }
                    }

                    $timeout(function () {
                        $scope.shiroExps = tempShiroExps;
                    });
                }

                this.checkNodesByShiroExps = checkNodesByShiroExps;

                function checkNodesByShiroExps() {

                    if (!angular.isArray($scope.shiroExps)) {
                        $scope.shiroExps = [];
                        return;
                    }
                    var ztree = $.fn.zTree.getZTreeObj($scope.ztreeId);
                    if (!ztree) return;

                    for (var idx in $scope.shiroExps) {
                        var exp = $scope.shiroExps[idx];
                        checkNodesByOneShiroExp(ztree, exp);
                    }
                }

                function checkNodesByOneShiroExp(ztree, exp) {

                    var parts = exp.split(":");
                    var parentNode = null;
                    for (var partIdx in parts) {
                        var isLeaf = (partIdx == parts.length - 1);

                        var part = parts[partIdx];
                        var tinyParts = isLeaf ? part.split(",") : [part];
                        var nodes = [];
                        if (parentNode == null) {
                            nodes = ztree.getNodes();
                        }
                        else {
                            nodes = parentNode.children;
                        }

                        if (!nodes || nodes.length == 0) {
                            //no children found！
                            return;
                        }

                        for (var nIdx in nodes) {
                            var node = nodes[nIdx];
                            if (!isLeaf) {
                                if (node.value == part) {
                                    parentNode = node;
                                    break;
                                }
                                continue;
                            }

                            if (part == "*") {
                                ztree.checkNode(parentNode, true, true);
                                break;
                            }
                            else {
                                if (tinyParts.indexOf(node.value) >= 0) {
                                    ztree.checkNode(node, true, true);
                                }
                            }
                        }
                    }
                }

                //为某个半选或者全选的节点构建shiro表达式
                function buildShiroExp(node, shiroExps) {
                    var path = node.getPath();
                    var exp = "";

                    for (var idx in path) {

                        var pathNode = path[idx];
                        if (pathNode.getCheckStatus().half && idx < path.length - 1) {
                            buildShiroExp(pathNode, shiroExps);
                        }
                        if (exp.length > 0) exp += ":";
                        exp += path[idx].value;
                    }
                    if (node.getCheckStatus().half) {
                        exp += ":r";
                    }

                    if (shiroExps.indexOf(exp) >= 0) return;
                    shiroExps.push(exp);
                }
            },
            link: function (scope, element, attrs, ctrl) {
                scope.$watch("shiroExps", function (newValue, oldValue) {
                    $timeout(function () {
                        ctrl.checkNodesByShiroExps();
                    });
                });
            }

        };

    }
);

'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */

angular.module('mdmApp')
    .directive('sidebarSearch', function () {
        return {
            templateUrl: 'directives/sidebar/sidebar-search/sidebar-search.html',
            restrict: 'E',
            replace: true,
            scope: {},
            controller: function ($scope) {
                $scope.selectedMenu = 'home';
            }
        }
    });

'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */

angular.module('mdmApp')
    .directive('sidebar', ['$location', '$timeout', 'subject', function ($location, $timeout, subject) {
        return {
            templateUrl: 'directives/sidebar/sidebar.html',
            restrict: 'E',
            replace: true,
            scope: {},
            controller: function ($scope) {
                $scope.selectedMenu = 'mdm';
                $scope.collapseVar = 0;
                $scope.multiCollapseVar = 0;
                $scope.check = function (x) {
                    if (x == $scope.collapseVar)
                        $scope.collapseVar = 0;
                    else
                        $scope.collapseVar = x;
                };
                $scope.multiCheck = function (y) {
                    if (y == $scope.multiCollapseVar)
                        $scope.multiCollapseVar = 0;
                    else
                        $scope.multiCollapseVar = y;
                };
            },
            link: function ($scope) {
                $scope.$watch(
                    function () {
                        return subject.authenticated;
                    },
                    function (newValue, oldValue) {
                    $timeout(function () {
                        $('#side-menu').metisMenu();
                        var url = window.location;
                        var element = $('#side-menu a').filter(function () {
                            return this.href == url.href.substr(0, url.href.indexOf('?') == -1 ? undefined : url.href.indexOf('?'))
                        });

                        element.parent().addClass('active').closest('ul').addClass('in').parent().addClass('active');

                        if (window.__selectMenuTargetUri) {
                            selectMenu(window.__selectMenuTargetUri);
                            delete window.__selectMenuTargetUri;
                        }
                    });
                });
                window.addEventListener('hashchange', function () {
                    var url = window.location;
                    var element = $('#side-menu a').filter(function () {
                        return this.href == url.href.substr(0, url.href.indexOf('?') == -1 ? undefined : url.href.indexOf('?'))
                    });
                    if (element.length == 0) {
                        return
                    }
                    $("#side-menu .active,#side-menu .in").each(function () {
                        $(this).removeClass('active');
                        $(this).removeClass('in');
                    });
                    element.parent().addClass('active').closest('ul').addClass('in').parent().addClass('active');
                })
            }
        }
    }]);



'use strict';

/**
 * @ngdoc directive
 * @name izzyposWebApp.directive:adminPosHeader
 * @description
 * # adminPosHeader
 */
angular.module('mdmApp')
    .directive('timeline', function () {
        return {
            templateUrl: 'directives/timeline/timeline.html',
            restrict: 'E',
            replace: true,
        }
    });

/**
 * Created by TeckyLee on 2016/5/16.
 */
angular.module('ngzTree', []).directive('zTree', ['$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            scope: {
                setting: "=",
                zNodes: "=",
                initialized: "=onInitialized"
            },
            link: function (scope, element, attrs) {
                $timeout(function () {
                    $.fn.zTree.init(element, scope[attrs.setting], scope[attrs.zNodes]);
                    if (scope.initialized) {
                        //触发onInitialized事件
                        scope.initialized();
                    }
                });

                scope.$watch("zNodes", function (newValue, oldValue) {
                    $timeout(function () {
                        $.fn.zTree.init(element, scope.setting, newValue);
                        if (scope.initialized) {
                            //触发onInitialized事件
                            scope.initialized();
                        }
                    });
                });
            }
        }
    }
])
