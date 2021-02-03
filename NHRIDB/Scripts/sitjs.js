(function ($) {
    $.fn.numcode = function (options, param) {
        var target = $(this);

        //options
        var methods = $.fn.numcode.methods;
        if (typeof options == 'string') {
            return methods[options](target, param);
        }
        options = $.extend({}, $.fn.numcode.defaults, options || {});

        if (!$(target)[0].id)
            throw 'id is required';

        $.fn.numcode.defaults = options; //for methods

        let id = $(target)[0].id;
        target.append('<div class="numcode_msg"></div>'
            + '<div class="numcode">'
            + '<canvas id="canvas' + id + '" width="100" height="30" ></canvas>'
            + '<input name="text' + id + '" type="text" />'
            + '</div>');

        methods["ShowCode"](target);



    };
    $.fn.numcode.defaults = {
        char: ['K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'U', 'V', 'W', 'X', 'Y', 'Z', '4', '5', '6', '7', '8', '9', '1', '2', '3'],
        codeLen: 4
    };

    $.fn.numcode.methods = {
        ShowCode: function (el) {
            let options = $.fn.numcode.defaults;
            let canvas = el.find('canvas')[0];
            let ctx = canvas.getContext('2d');

            var oImg = new Image();
            oImg.src = '../images/bg.jpg';
            oImg.onload = function () {
                var w = this.width,
                    h = this.height;
                ctx.drawImage(this, 0, 0, w, h);
                ctx.font = "18px sans-serif";
                let x = 0;
                let code = [], code2 = [];
                for (let i = 0; i < options.codeLen; i++) {
                    var charIndex = Math.floor(Math.random() * options.char.length);
                    var charIndex2 = Math.floor(Math.random() * options.char.length);
                    code.push(options.char[charIndex]);
                    code2.push(options.char[charIndex2]);

                    ctx.fillStyle = '#000';
                    ctx.fillText(code[i], x, 20);
                    ctx.fillStyle = '#ef1515';
                    x = x + 11;
                    ctx.fillText(code2[i], x, 20);
                    x = x + 11;
                }//end for
                options.code = code;
                options.code2 = code2;
            }//end load

            options.type = Math.floor(Math.random() * 2);
            let label = el.find(".numcode_msg");
            switch (options.type) {
                case 0:
                    label.html("輸入黑色字驗證碼");
                    break;
                case 1:
                    label.html("輸入紅色字驗證碼");
                    break;
            }
        },
        CheckCode: function (el) {
            let options = $.fn.numcode.defaults;
            let checkCode = options.type == 0 ? options.code : options.code2;
            let value = el.find("input")[0].value;
            if (options.hideInputId) {
                $("#" + options.hideInputId).val("1" + checkCode.toString());
            }
            for (let i = 0; i < options.codeLen; i++) {
                if (value[i] != checkCode[i]) {
                    this.ShowCode(el);
                    return false;
                }
            }//end for

            return true;
        }
    }
})(jQuery);
 


