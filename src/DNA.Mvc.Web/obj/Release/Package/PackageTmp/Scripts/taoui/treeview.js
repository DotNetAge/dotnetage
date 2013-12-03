/// <reference path="../jquery-1.4.4.js" />
/// <reference path="../jquery-1.4.4-vsdoc.js" />

/*!  
** Copyright (c) 2011 Ray Liang (http://www.dotnetage.com)
** Dual licensed under the MIT and GPL licenses:
** http://www.opensource.org/licenses/mit-license.php
** http://www.gnu.org/licenses/gpl.html
** 
**----------------------------------------------------------------
** title        : DJME TreeView
** version   : 3.0.0
** modified: 2012-2-27
** depends:
**    jquery.ui.core.js
**    jquery.ui.widget.js
**    jquery.ui.mouse.js
**    jquery.ui.draggable.js
**    jquery.ui.droppable.js
**    jquery.tmpl.js
**    editable.js
**----------------------------------------------------------------
*/
(function ($) {
    $.widget("dna.taoTreeview", $.dna.taoHierarchical, {
        options: {
            enableDropAndDrag: false,
            singlePathExpand: false,
            checkboxes: false,
            showTreeLines: true,
            unselectable: false,
            dragstart: null,
            dropTargets: null,
            drag: null,
            dragstop: null,
            dropped: null,
            dropover: null,
            dropout: null,
            selected: null,
            unselected: null,
            selectedNode: null,
            checked: null,
            collapsed: null,
            expanded: null,
            beforeNodeLoad: null,
            nodeLoadError: null,
            nodeLoaded: null,
            popupNodes: null,
            popupAttr: "url",
            nodeClass: null,
            show: {
                effect: "drop",
                options: { direction: "up" },
                speed: 200
            }, hide: {
                effect: "slide",
                options: { direction: "up" },
                speed: 200
            }
        },
        _create: function () {
            var self = this;
            this._unobtrusive()
                   .element.addClass("d-reset d-ui-widget d-tree");

            this._initItems()
                   ._bindEvents()
                   ._setDataSource(this.options.datasource);

            if (this.options.autoBind)
                this.databind(function (results) {
                    self._createDataElements(results.data);
                });

            this.element.css("opacity", "1");
            $(".d-node", this.element).disableSelection();

            if (this.options.unselectable) {
                this.element.bind("click", function () {
                    $(".d-node.d-state-active", this.element).removeClass("d-state-active");
                    $(".d-node.d-state-hover", this.element).removeClass("d-state-hover");
                    $(".d-node-content.d-state-active", this.element).removeClass("d-state-active");
                    $(".d-node-content.d-state-hover", this.element).removeClass("d-state-hover");
                    self._triggerEvent("unselected");
                })
            }

            this.element.bind("mouseleave", function () { $(".d-state-hover", self.element).removeClass("d-state-hover"); });
        },
        _unobtrusive: function () {
            var $t = this.element, opts = this.options;
            if ($t.data("drag-drop")) opts.enableDropAndDrag = $t.dataBool("drag-drop");

            if ($t.data("singlepath")) opts.singlePathExpand = $t.dataBool("singlepath");
            if ($t.data("checkboxes") != undefined) opts.checkboxes = $t.dataBool("checkboxes");
            if ($t.data("unselectable") != undefined) opts.unselectable = $t.dataBool("unselectable");

            if ($t.data("source"))
                opts.datasource = $t.datajQuery("source");
            else {
                if ($t.data("source-url")) {
                    var _srcUrl = $t.data("source-url");
                    opts.datasource = {
                        actions: { read: _srcUrl },
                        mapper: _srcUrl.endsWith(".xml") ? new tao.xmlMapper() : new tao.mapper()
                    };
                }
            }

            if ($t.data("tmpl"))
                opts.itemTmpl = $t.datajQuery("tmpl");

            if ($t.data("autobind") != undefined) opts.autoBind = $t.dataBool("autobind");

            if ($t.data("onselected")) opts.selected = new Function("event", "ui", $t.data("onselected"));
            if ($t.data("selected")) opts.selected = new Function("event", "ui", $t.data("selected"));

            if ($t.data("unselected"))
                opts.unselected = new Function("event", $t.data("unselected"));

            if ($t.data("checked")) opts.checked = new Function("event", "ui", $t.data("checked"));

            if ($t.data("drag")) opts.drag = new Function("event", "ui", $t.data("drag"));
            if ($t.data("drag-stop")) opts.dragstop = new Function("event", "ui", $t.data("drag-stop"));
            if ($t.data("drag-start")) opts.dragstart = new Function("event", "ui", $t.data("drag-start"));
            if ($t.data("drop-out")) opts.dropout = new Function("event", "ui", $t.data("drop-out"));
            if ($t.data("drop-over")) opts.dropover = new Function("event", "ui", $t.data("drop-out"));
            if ($t.data("dropped")) opts.dropped = new Function("event", "ui", $t.data("dropped"));
            if ($t.data("collapsed")) opts.collapsed = new Function("event", "ui", $t.data("collapsed"));
            if ($t.data("expanded")) opts.expanded = new Function("event", "ui", $t.data("expanded"));

            if ($t.data("before-load")) opts.beforeNodeLoad = new Function("event", "ui", $t.data("before-load"));
            if ($t.data("load-error")) opts.nodeLoadError = new Function("event", "ui", $t.data("load-error"));
            if ($t.data("popup")) opts.popupNodes = new Function("event", "ui", $t.data("popup"));
            if ($t.data("popup-attr")) opts.popupAttr = $t.data("popup-attr");
            if ($t.data("node-class")) opts.nodeClass = $t.data("node-class");
            return this;
        },
        _bindEvents: function () {
            var eventPrefix = this.widgetEventPrefix;
            if (this.options.selected)
                this.element.bind(eventPrefix + "selected", this.options.selected);

            if (this.options.unselected)
                this.element.bind(eventPrefix + "unselected", this.options.unselected);

            if (this.options.drag)
                this.element.bind(eventPrefix + "drag", this.options.drag);

            if (this.options.dragstop)
                this.element.bind(eventPrefix + "dragstop", this.options.dragstop);

            if (this.options.dragstart)
                this.element.bind(eventPrefix + "dragstart", this.options.dragstart);

            if (this.options.dropout)
                this.element.bind(eventPrefix + "dropout", this.options.dropout);

            if (this.options.dropover)
                this.element.bind(eventPrefix + "dropover", this.options.dropover);

            if (this.options.dropped)
                this.element.bind(eventPrefix + "dropped", this.options.dropped);

            if (this.options.collapsed)
                this.element.bind(eventPrefix + "collapsed", this.options.collapsed);

            if (this.options.expanded)
                this.element.bind(eventPrefix + "expanded", this.options.expanded);

            if (this.options.checked)
                this.element.bind(eventPrefix + "checked", this.options.checked);

            if (this.options.beforeNodeLoad)
                this.element.bind(eventPrefix + "beforeNodeLoad", this.options.beforeNodeLoad);

            if (this.options.nodeLoadError)
                this.element.bind(eventPrefix + "nodeLoadError", this.options.nodeLoadError);

            if (this.options.nodeLoaded)
                this.element.bind(eventPrefix + "nodeLoaded", this.options.nodeLoaded);

            if (this.options.popupNodes)
                this.element.bind(eventPrefix + "popupNodes", this.options.popupNodes);

            return this;
        },
        _setOption: function (key, value) {
            if (key == "selectedNode") {
                if (value)
                    this.select(value);
                return this;
            }

            return $.dna.taoDataBindingList.prototype._setOption.call(this, key, value);
        },
        _initItem: function (n) {
            var self = this, opts = self.options, $li = $(n);

            $li.addClass("d-node")
                .disableSelection()
                .wrapInner("<div class='d-node-content'></div>");

            if (opts.nodeClass)
                $li.addClass(opts.nodeClass);

            var _child = $(">.d-node-content>ul", $li);

            if (_child.length) {
                $li.addClass("d-node-hasChildren")
                     .append(_child);
                _child.wrap("<div class='d-nodes-holder'/>");
            }

            if ($li.data(opts.popupAttr)) {
                if (!$li.hasClass("d-node-hasChildren"))
                    $li.addClass("d-node-hasChildren");
                $li.addClass("d-state-collapse");
            }

            if ($li.data("disabled")) {
                $li.isDisable(true) // .addClass("d-state-disable")
                    .removeAttr("data-disabled");
            }

            self._createNodeButton($li);

            $li.children(".d-node-content")
                 .click(function (event) {
                     event.stopPropagation();
                     if (!$li.isDisable()) //hasClass("d-state-disable"))
                         self.select($li);
                 })
                 .dblclick(function (event) {
                     self._toggle($li);
                 })
                 .hover(function (event) {
                     if (!$li.isDisable()) {
                         $(".d-node>.d-node-content>a.d-state-hover", self.element).removeClass("d-state-hover");
                         $(".d-node>.d-node-content.d-state-hover", self.element).removeClass("d-state-hover");

                         if ($(">.d-node-content>a", $li).length > 0)
                             $(">.d-node-content>a", $li).isHover(true);
                         else
                             $(">.d-node-content", $li).isHover(true);
                     }
                 });

            if (self.options.checkboxes)
                self._enableCheckboxs($li);

            var img = $(".d-node-content>a>img", $li);
            if (img.length) {
                img.addClass("d-node-img");
                var a = $(">.d-node-content>a", $li);
                a.before(img);
            }

            if (self.options.enableDropAndDrag)
                self._enableDropAndDrag(n);

            if ($li.data("expanded")) {
                $li.addClass("d-state-expand")
                    .removeClass("d-state-collapse")
                    .removeAttr("data-expanded");

                if ($li.hasClass("d-node-hasChildren"))
                    $(">.d-node-button", $li).empty().append($("<span/>").addClass("d-icon-caret-down"));
            }
            else {
                if ($li.hasClass("d-node-hasChildren"))
                    $li.removeClass("d-state-expand")
                        .addClass("d-state-collapse");

                if ($li.hasClass("d-node-hasChildren"))
                    $(">.d-node-button", $li).empty().append($("<span/>").addClass("d-icon-caret-right"));
            }

            if ($li.data("selected")) {
                var _link = $(">.d-node-content>a", $li);
                if (_link.length) _link.isActive(true);
                else
                    $li.children(".d-node-content").isActive(true);
            }
        },
        _popupAttrs: function (key, val, item) {
            if (key == "expanded") {
                if (val) {
                    item.addClass("d-state-expand")
                            .removeClass("d-state-collapse")
                            .removeAttr("data-expanded");
                }
                else {
                    if (item.hasClass("d-node-hasChildren"))
                        item.removeClass("d-state-expand")
                                .addClass("d-state-collapse");
                }
                return true;
            }
            return false;
        },
        _enableCheckboxs: function ($li) {
            var self = this, cbox = $("<input type='checkbox'>").addClass("d-node-checkbox");
            $li.children(".d-node-content").before(cbox);
            cbox.click(function () {
                //var cbs = $li.find(".d-node-checkbox");
                //if (cbs.length > 0) {
                //    cbs.each(function (i, n) {
                //        $(n).attr("checked", cbox.attr("checked") ? true : false);
                //    });
                //}

                var _parent = $li.parents(".d-node");

                if (_parent.length) {
                    //var _vals = 0;
                    //$li.siblings().each(function (i, n) {
                    //    if ($(n).children(".d-node-checkbox").attr("checked") == cbox.attr("checked"))
                    //        _vals++;
                    //});

                    //if (_vals == $li.siblings().length)
                    //    _parent.children(".d-node-checkbox").attr("checked", cbox.attr("checked"));

                    self._triggerEvent("checked", { node: $li, checked: cbox.attr("checked") });
                }
            });
        },
        _enableDropAndDrag: function (_node) {
            var self = this, opts = this.options;

            $(_node).children(".d-node-content").draggable({
                revert: 'invalid',
                greedy: false,
                iframeFix: true,
                helper: 'clone',
                opacity: 0.8,
                start: function (event, ui) {
                    self._triggerEvent("dragstart", $(this).closest(".d-node"));
                },
                stop: function (event, ui) {
                    self._triggerEvent("dragstop", $(this).closest(".d-node"));
                },
                drag: function (event, ui) {
                    self._triggerEvent("drag", $(this).closest(".d-node"));
                }
            });

            if (opts.dropTargets) {
                var _targets = $(opts.dropTargets);
                if (_targets.length) {
                    _targets.droppable({
                        accept: ".d-node-content",
                        over: function (event, ui) {
                            self._triggerEvent("dropout", { node: ui.draggable, container: $(this) });
                        },
                        out: function (event, ui) {
                            self._triggerEvent("dropover", { node: ui.draggable, container: $(this) });
                        },
                        drop: function (event, ui) {
                            self._triggerEvent("dropped", { node: ui.draggable, container: $(this) });
                        }
                    });
                }
            }
            //drop to it node
            $(_node).droppable({
                accept: ".d-node-content",
                greedy: true,
                out: function (event, ui) {
                    self._triggerEvent("dropout", { node: ui.draggable, targetNode: $(this).closest(".d-node") });
                },
                over: function (event, ui) {
                    self._triggerEvent("dropover", { node: ui.draggable, targetNode: $(this).closest(".d-node") });
                },
                drop: function (event, ui) {
                    var _srcNode = ui.draggable.closest(".d-node"),
                    _srcParent = _srcNode.closest(".d-node-hasChildren"),
                     thisNode = $(this).closest(".d-node");

                    if (_node === _srcNode[0]) return false; //drop on self

                    if (_srcNode.has(thisNode).length) return false;

                    var _holder = $(">.d-nodes-holder", this);

                    if (_holder.length === 0) {
                        _holder = $("<ul/>").addClass("d-nodes-holder");
                        $(this).append(_holder);
                        thisNode.addClass("d-node-hasChildren");
                    }
                    _holder.append(_srcNode);

                    if (_srcParent.length) {
                        if (_srcParent.find(".d-node").length === 0) {
                            _srcParent.removeClass("d-node-hasChildren")
                                       .children(".d-nodes-holder").remove();
                        }
                    }

                    var _pos = 0;
                    var _children = $(this).find(".d-node");
                    if (_children.length)
                        _pos = _children.index(_srcNode);
                    self._triggerEvent("dropped", { node: _srcNode, targetNode: thisNode, position: _pos });
                }
            });
        },
        _createNodeButton: function ($li) {
            var self = this,
            _tbtn = $("<div/>").addClass("d-node-button")
                                              .prependTo($li);

            _tbtn.click(function (event) {
                event.stopPropagation();
                if ($li.hasClass("d-node-hasChildren"))
                    self._toggle($li);
            });
        },
        _toggle: function ($li) {
            if ($li.hasClass("d-state-collapse"))
                this.expand($li);
            else
                this.collape($li);
        },
        _onInserted: function (val) { ///<summary>从DataSource 接收到 inserted 事件的默认处理函数</summary>
            if (val) {
                this.add(val, this.options.selectedNode);
            }
        },
        _onUpdated: function (val) { ///<summary>通过数据项更新元素内容</summary>  
            if (val) {
                var node = this.options.selectedNode,
                _tmplItem = $(">.d-node-content", node).children().tmplItem();
                if (_tmplItem.data) {
                    _tmplItem.data = val.result;
                    _tmplItem.update();
                    //Update data items
                    var _d = _tmplItem.data;
                    for (var p in _d) {
                        if (p == "title")
                            node.attr("title", _d[p]);
                        else
                            node.attr("data-" + p, _d[p]);
                    }
                }
            }
        },
        select: function ($li) {
            var opts = this.options, $target = $li, el = this.element;

            if ($li == null)
                $target = el.children(".d-node").first();

            if ($target.length) {
                var link = $(">.d-node-content>a", $target);

                $(".d-node>.d-node-content>a.d-state-active", el).removeClass("d-state-active");
                $(".d-node>.d-node-content>a.d-state-hover", el).removeClass("d-state-hover");
                $(".d-node>.d-node-content.d-state-active", el).removeClass("d-state-active");
                $(".d-node>.d-node-content.d-state-hover", el).removeClass("d-state-hover");

                if (link.length) {
                    link.isActive(true);
                }
                else {
                    $(">.d-node-content", $target).isActive(true);
                }
                opts.selectedNode = $target;
                this._setPosition($target)
                       ._triggerEvent("selected", { node: $target, dataItem: $target.data("dataItem") });
            }
        },
        collape: function ($li) {
            var self = this, opts = this.options;
            if ($li.hasClass("d-state-collapse")) return;
            $li.removeClass("d-state-expand")
            if ($li.hasClass("d-node-hasChildren")) {
                $(">.d-nodes-holder", $li).stop().hide(opts.hide.effect, opts.hide.options, opts.hide.speed, function () {
                    $li.addClass("d-state-collapse");
                    self._triggerEvent("collapsed", $li);
                });
                $(">.d-node-button", $li).empty().append($("<span/>").addClass("d-icon-caret-right"));
            }
        },
        expand: function ($li) {
            var self = this, opts = this.options;
            if ($li.hasClass("d-state-expand")) return;

            if ($li.data(opts.popupAttr) && this._source) {
                var icon = $(">.d-node-button", $li).empty().append($("<span/>").addClass("d-icon-loading"));
                // On popuplate items
                this._read($li.data(opts.popupAttr))
                       .done(function (results) {
                           var w = $("<div class='d-nodes-holder' />").appendTo($li);
                           self.addNodes(results.data, $("<ul/>").appendTo(w));
                           $li.removeData(opts.popupAttr)
                               .removeAttr("data-" + opts.popupAttr);
                           //$(">.d-node-button", $li).removeClass("d-icon-loading");
                           $(">.d-node-button>.d-icon-loading", $li).remove();
                           if (!results.total)
                               $li.removeClass("d-node-hasChildren");

                           self._triggerEvent("popupNodes", $li);
                           self.expand($li);
                       });
            } else {
                $li.removeClass("d-state-collapse");
                if ($li.hasClass("d-node-hasChildren")) {
                    $li.addClass("d-state-expand");
                    $(">.d-nodes-holder", $li).stop().show(opts.show.effect, opts.show.options, opts.show.speed);

                    $(">.d-node-button", $li).empty().append($("<span/>").addClass("d-icon-caret-down"));

                    if (this.options.singlePathExpand) {
                        var _siblings = $li.siblings().not(".d-state-disable");
                        if (_siblings.length) _siblings.each(function (i, n) {
                            self.collape($(n));
                        });
                    }
                    this._triggerEvent("expanded", $li);
                }

            }
        },
        addNodes: function (data, parent) {
            this._createDataElements(data, parent);
            return this;
        },
        add: function (nodeData, parentNodeElement) {
            var p = null;
            if (parentNodeElement != null) {
                p = parentNodeElement.find(".d-nodes-holder>ul");
                if (p.length == 0)
                    p = $("<div class='d-nodes-holder'><ul></ul></div>").appendTo(parentNodeElement).children();
            }
            $li = this._addItem(nodeData, p);
            this._initItem($li);
            if (parentNodeElement)
                this.expand(parentNodeElement.addClass("d-node-hasChildren"));
            this.select($li);
            return $li;
        },
        getCheckedNodes: function () {
            if (this.options.checkboxes) {
                var _checkedboxs = this.element.find(".d-node-checkbox");
                if (_checkedboxs.length) {
                    var nodeArgs = new Array();
                    _checkedboxs.each(function (i, n) {
                        if ($(n).attr("checked")) {
                            nodeArgs.push($(n).parent().closest(".d-node"));
                        }
                    });
                    return nodeArgs;
                }
            }
            return null;
        },
        getCheckedAttrs: function (attrName) {
            var keys = [], nodes = this.getCheckedNodes();
            $.each(nodes, function (i, n) {
                keys.push($(n).attr("data-id"));
            });
            return keys;
        },
        getCheckedTexts: function () {
            var keys = [], nodes = this.getCheckedNodes();
            $.each(nodes, function (i, n) {
                keys.push($(n).children(".d-treenode-content").find(".d-treenode-text").text());
            });
            return keys;
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);
