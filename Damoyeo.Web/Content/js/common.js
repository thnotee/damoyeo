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


/**
 * 
 * @param {any} meetup_id 소모임 ID
 */
function SetWishList(heart_btn, meetup_id) {

    console.log(heart_btn);
    
    $.ajax({
        type: "POST",
        url: "/User/UpsertWish",
        data: { meetup_id: meetup_id },
        success: function (response) {
            console.log(response)
            if (response.success) {
                if (response.code == 1) {
                    alertBox("찜목록 추가 되었습니다.");
                    $(heart_btn).find("img").attr("src", "/Content/images/heart_on.svg");
                }
                else if (response.code == 2) {
                    alertBox("찜목록에서 삭제하였습니다.");
                    $(heart_btn).find("img").attr("src", "/Content/images/heart.svg");
                }
                
            }
            else
            {
                alertBox("로그인이 필요한 서비스입니다.");
            }
        },
        error: function (xhr, status, error) {
            console.error(xhr);
            
        }
    });
    
}

//하트 클릭시 버튼 색 변함
$(document).ready(function () {
    $(".heart_click").click(function () {

        var meetup_id = $(this).data("meetup_id");
        //console.log(meetup_id);
        SetWishList($(this), meetup_id);
        /*
        $(this).find("img").attr("src", function (_, attr) {
                return attr === "/Content/images/heart.svg"
                    ? "/Content/images/heart_on.svg"
                    : "/Content/images/heart.svg";
            });*/
    });

    $(".top_fixed").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1000);
        return false;
    });
});


