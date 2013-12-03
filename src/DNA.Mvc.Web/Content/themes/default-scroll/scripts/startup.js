$(function () {
    var checkPoints = [], startPoint = null, currentCheckPointIndex = 0;
    var findCheckPoint = function (_top) {
        for (var i = 0; i < checkPoints.length; i++) {
            var cp = checkPoints[i],
                _sp = cp.startPoint.y - ($(cp.element).height() * 0.6),
                _ep = cp.endPoint.y - ($(cp.element).height() * 0.6);

            if (_top >= _sp && _top <= _ep)
                return cp;
        }
    },
    _initCheckPoints = function () {
        checkPoints = [];
        $('.d-page').children('.d-page-part:visible').each(function (i, n) {
            var _offset = $(n).offset();
            checkPoints.push({
                index: i,
                element: n,
                startPoint: {
                    y: _offset.top,
                    x: _offset.left
                },
                endPoint: {
                    y: $(n).next().length ? $(n).next().offset().top : (_offset.top + $(n).outerHeight(true)),
                    x: _offset.left
                }
            });
        });
    },
        _init = function () {
            _initCheckPoints();

            if (checkPoints.length > 1) {
                var scrollerNav = $("<ul/>").addClass("d-scroller-nav").appendTo($("body")),
                    sections = $('.d-page').children('.d-page-part');
                sections.each(function (i, n) {
                    var _section = $("<li/>").appendTo(scrollerNav)
                              .click(function () {
                                  window.scrolling = true;
                                  $("body,html").animate({ scrollTop: checkPoints[i].startPoint.y + 1 }, function () {
                                      window.scolling = false;
                                  });
                              });
                });

                startPoint = findCheckPoint($(window).scrollTop());
                currentCheckPointIndex = startPoint ? startPoint.index : 0;
                //console.log(currentCheckPointIndex);

                if (currentCheckPointIndex === 0) {
                    $(checkPoints[0].element).trigger("enterview", checkPoints[0]);
                } else {
                    $(checkPoints[0].element).trigger("leaveview", checkPoints[0]);
                    $(checkPoints[currentCheckPointIndex].element).trigger("enterview", checkPoints[currentCheckPointIndex]);
                }
            }
        };

    $(window).bind("scroll", function () {
        if (checkPoints.length <= 1) return;

        var checkPointIndex = findCheckPoint($(window).scrollTop()).index;

        if (checkPointIndex === currentCheckPointIndex) {
            //In range
        }
        else {
            //Reached
            var prevCheckPoint = checkPoints[currentCheckPointIndex],
                _checkPoint = checkPoints[checkPointIndex];
            currentCheckPointIndex = checkPointIndex;
            $(prevCheckPoint.element).trigger("leaveview", prevCheckPoint);
            $(_checkPoint.element).trigger("enterview", _checkPoint);
            if (window.scrolling)
                return;
            window.scrolling = true;
            $("body,html").animate({ scrollTop: checkPoints[currentCheckPointIndex].startPoint.y + 1 }, function () {
                window.scrolling = false;
            });
            //console.log("Enter:" + _checkPoint.index+ " Leave:"+prevCheckPoint.index);
        }
    });

    $(window).bind("resize", function () {
        _initCheckPoints();
    });

    $('.d-page').children('.d-page-part').each(function (i, n) {
        if ($(n).find(".d-widget").length === 0) { $(n).hide();}
    });

    $(document).bind("designpage", function (event,mode) {
        if (mode === "contents")
        {
            $('.d-page').children('.d-page-part').each(function (i, n) {
                if ($(n).find(".d-widget").length === 0) { $(n).hide(); }
            });
        }
        else
            $('.d-page').children('.d-page-part').show();
    });

    $('.d-page').children('.d-page-part:visible').on("enterview", function (event, checkpoint) {

        var _bgColor = $(this).css("background-color"), _rgba = "";
        if (_bgColor.startsWith("rgb"))
            _rgba = $.hex2rgb($.rgb2hex(_bgColor), .9);
        else
            _rgba = $.hex2rgb(_bgColor, .9);

        $(".d-sitetools").css({ "background-color": _rgba });

        $(".d-scroller-nav").children().eq(checkpoint.index).isActive(true);
        $(this).addClass("d-state-reached");

    }).on("leaveview", function (event, checkpoint) {
        $(this).removeClass("d-state-reached");
        $(".d-scroller-nav").children().eq(checkpoint.index).isActive(false);
        //$("body,html").animate({ scrollTop: checkPoints[checkpoint.index + 1].startPoint.y + 1 });
    });

    //Lazy init
    window.setTimeout(function () {
        _init();
    }, 500);

    $("footer").appendTo($(".d-page"));

});
