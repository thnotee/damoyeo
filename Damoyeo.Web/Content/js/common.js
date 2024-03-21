/**
 * 텍스트 표출 및 focus 준다
 * @param {any} text
 * @param {any} obj
 */
function alertBox(text, func) {

    if (func == null) {
        Swal.fire({
            title: text
        });
    }
    else
    {
        Swal.fire({
            title: text
        }).then((result) => {
            if (result.isConfirmed) {
                // 여기에 확인 버튼 클릭 시 실행할 함수를 작성하세요.
                func();
            }
        });
    }

}

function reloadPage()
{
    location.reload();
}

/**
 * 텍스트 표출 및 focus 준다
 * @param {any} text
 * @param {any} obj
 */
function alertFocus(text, obj) {

    Swal.fire({
        title: text,
        didClose: () => {
            obj.focus();
        }
    });
}

//하트 클릭시 버튼 색 변함
$(document).ready(function () {
    $(".heart_click").click(function () {
        $(this)
            .find("img")
            .attr("src", function (_, attr) {
                return attr === "/Content/images/heart.svg"
                    ? "/Content/images/heart_on.svg"
                    : "/Content/images/heart.svg";
            });
    });

    $(".top_fixed").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1000);
        return false;
    });
});
