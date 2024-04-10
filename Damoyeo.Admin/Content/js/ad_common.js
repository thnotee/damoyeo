$(document).ready(function () {
  $("a.on img").each(function () {
    var src = $(this).attr("src");
    if (!src.includes("_on")) { // "_on"이 이미 포함되어 있지 않다면 추가
      $(this).attr("src", src.replace(".svg", "_on.svg"));
    }
  });

  // hover 이벤트
  $(".left_menu a").hover(
    function () {
      // "on" 클래스가 없을 때만 실행
      if (!$(this).hasClass("on")) {
        $(this).find("img").attr(
          "src",
          $(this).find("img").attr("src").replace(".svg", "_on.svg")
        );
      }
    },
    function () {
      // "on" 클래스가 없을 때만 실행
      if (!$(this).hasClass("on")) {
        $(this).find("img").attr(
          "src",
          $(this).find("img").attr("src").replace("_on.svg", ".svg")
        );
      }
    }
  );
});


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
    else {
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

function reloadPage() {
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
