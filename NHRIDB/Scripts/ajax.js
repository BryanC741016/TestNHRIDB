var msg;
function ajaxFromDataByTonken(url, postdata, tonken, fun, el) {

    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        processData: false,
        contentType: false,
        // dataType: 'json',
        timeout: 3600000,
        headers: {
            'RequestVerificationToken': tonken,
            //"X-CSRFtoken": $.cookie("csrftoken"),
          //   'Access-Control-Allow-Headers': 'content-type',
          //  'Access-Control-Allow-Origin': '*' 
        },
        success: function (result) {
            if (typeof fun === 'function') {
                $(".loading").css("visibility", "hidden");
                fun(result,el);
            } else {
                if (result.isSuccess) {
                    location.reload();
                } else {
                    $(".loading").css("visibility", "hidden");
                    showMessage(result.message);
                }
            }
           
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $(".loading").css("visibility", "hidden");
            showMessage("傳送資料發生錯誤");
        }
    })
}
function ajaxFromDataByTonkenDone(url, postdata, tonken, fun, el) {

    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        processData: false,
        contentType: false,
        timeout: 3600000,
        headers: {
            'RequestVerificationToken': tonken,
        
        },
    }).done(function (data, textStatus, jqXHR) {
        fun(data, el)
    }).always(function (xx) {
        $(".loading").css("visibility", "hidden");
    });
}
function ajaxJsonByTonkenDone(url, postdata, tonken, fun, el) {
    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        dataType: 'json',
        timeout: 3600000,
        headers: {
            'RequestVerificationToken': tonken,
        }
    }).done(function (data, textStatus, jqXHR) {
        fun(data,el)
    }).always(function (xx) {
        $(".loading").css("visibility", "hidden");
    });
}
function ajaxJsonByTonken(url, postdata, tonken, fun, el) {
    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        dataType: 'json',
       // contentType: 'application/x-www-form-urlencoded; charset=UTF-8', 
        timeout: 3600000,
        headers: {
            'RequestVerificationToken': tonken,
            //"Content-type":"application/json"
        },
        success: function (result) {
            if (typeof fun === 'function') {
                fun(result, el);
                $(".loading").css("visibility", "hidden");
            } else {
                if (result.isSuccess) {
                    location.reload();
                } else {
                    $(".loading").css("visibility", "hidden");
                    showMessage(result.message);

                }
            }
          

        },
        error: function (xhr, ajaxOptions, thrownError) {
            $(".loading").css("visibility", "hidden");
            showMessage("傳送資料發生錯誤");
        }
    })
}


/* 執行ajax */
function ajaxPost(url, postdata) {
    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        processData: false,
        contentType: false,
        // dataType: 'json',
        timeout: 3600000,
        success: function (result) {
           
            if (result == "yes") {
                location.reload();
            } else {
                $(".loading").css("visibility", "hidden");
                showMessage(result);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $(".loading").css("visibility", "hidden");
            showMessage("傳送資料發生錯誤");
        }
    })
}


function ajaxActByPost(url, postdata, successFunc, el) {
    $.ajax({
        url: url,
        method: 'POST',
        data: postdata,
        processData: false,
        contentType: false,
        // dataType: 'json',
        timeout: 3600000,
        success: function (result) {
          
            successFunc(result, el);
            $(".loading").css("visibility", "hidden");
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $(".loading").css("visibility", "hidden");
            showMessage("傳送資料發生錯誤");
        }
    })
}
/* 以下為 ajax 傳輸過程處理 */

//在ajax送資料過程中，顯示資料傳輸中
$(document).ajaxSend(function (e, xhr, opt) {
  //  block = new ajaxLoader( );
    $(".loading").css("visibility", "visible");
}).ajaxStop(function () {
     $(".loading").css("visibility", "hidden");
   // block.remove();
});



//取消ajax的快取機制
$.ajaxSetup({ cache: false });

function ajaxLoader( ) {
    // Becomes this.options

 
    this.init = function () {
       
        this.remove();
        $(".loading").css("visibility", "visible");

        $.ajaxSetup({
            statusCode: {
                440: function () {
                    window.Location.href = "./Home/Index";
                }
            }
        });
  
    };

    this.remove = function () {
   
        $(".loading").css("visibility", "hidden");
    }
    this.init();
}

