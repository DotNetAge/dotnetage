/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.splitter", {
        options: {
            paneA: null,
            paneB: null,
            autoSize: true,
            minWidth: 250,
            fillSpace: true
        },
        _create: function () {
            var self = this, opts = this.options, el = this.element, $resizer = $("td.d-splitter-helper:first", el),
            $a = $(opts.paneA), $b = $(opts.paneB);
            $a.width(300);
            var $helper = $("<div/>").addClass("d-splitter-helper")
                                .css({
                                    //height: $a.height() + "px",
                                    //width: $resizer.width() + "px",
                                    top: "0px",
                                    left: $resizer.position().left
                                })
                               .appendTo(el);

            $resizer.bind("mouseenter", function () {
                if ($helper.height() != $resizer.height()) {
                    $helper.height($resizer.height());
                }
            });

            $helper.draggable({ axis: "x",
                start: function (event, ui) {
                    $(this).css({ "opacity": "0.5" });
                    height: $resizer.height() + "px"
                },
                stop: function (event, ui) {
                    $(this).css({ "opacity": "1" });
                    $a.width(ui.position.left -2);
                }
            });
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);   