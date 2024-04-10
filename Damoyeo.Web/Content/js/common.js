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
    });

    $(".top_fixed").click(function () {
        $("html, body").animate({ scrollTop: 0 }, 1000);
        return false;
    });


    /**
     * 선택 이미지 프리뷰
     */
    $("#user_img").on("change", function () {

        var file = this.files[0]; // 선택된 파일 가져오기

        // 파일 타입 확인 (이미지인지)
        if (file && file.type.match('image.*')) {
            var formData = new FormData();
            formData.append('file', file); // 'file'은 서버에서 해당 파일을 식별하는 키입니다.

            // AJAX 요청
            $.ajax({
                url: '/User/ChangeProfileImage', // 서버 엔드포인트 URL
                type: 'POST',
                data: formData,
                processData: false, // processData와 contentType을 false로 설정해야 합니다.
                contentType: false,
                success: function (response) {
                    //console.log('업로드 성공', response);
                    // 업로드 성공 시 로직
                    if (response.success) {
                        //console.log(response.data);
                        $('#preview').attr('src', response.data);
                    }
                    else
                    {
                        alertBox(response.data)
                    }
                },
                error: function (xhr, status, error) {
                    console.log('업로드 실패', error);
                    // 업로드 실패 시 로직
                }
            });
        } else {
            alertBox('이미지 파일만 업로드 가능합니다.');
        }

    });
});



// 모바일 메뉴
$(function () {
    $(".hamburger").click(function () {
        $(".mo_side_menu").css("right", "0");
        $(".mo_side_dim").fadeIn();
        $("body").addClass("hidden");
    });
    $(".mo_close").click(function () {
        $(".mo_side_menu").css("right", "-100%");
        $(".mo_side_dim").fadeOut();
        $("body").removeClass("hidden");
    });
});

//마이페이지 상단 메뉴명 on붙은 부분 보여지게

/*
$(document).ready(function () {
    var activeItem = $('.my_move .on');
    var activeItemOffset = activeItem.closest('li').position().left;
    $('.my_move ul').scrollLeft(activeItemOffset - activeItem.width());
});*/