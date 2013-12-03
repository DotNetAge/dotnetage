/// <reference path="../jquery-1.4.1-vsdoc.js" />
///  Copyright (c) 2012 Ray Liang (http://www.dotnetage.com)
///  Dual licensed under the MIT and GPL licenses:
///  http://www.opensource.org/licenses/mit-license.php
///  http://www.gnu.org/licenses/gpl.html

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