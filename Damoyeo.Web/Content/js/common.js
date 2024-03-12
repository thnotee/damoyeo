/**
 * 텍스트 표출 및 focus 준다
 * @param {any} text
 * @param {any} obj
 */
function alertBox(text) {

    Swal.fire({
        title: text
    });
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
                return attr === "images/heart.svg"
                    ? "images/heart_on.svg"
                    : "images/heart.svg";
            });
    });

    $(".top_fixed").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1000);
        return false;
    });
});
