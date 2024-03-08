$(document).ready(function () {
  $(".left_menu a").hover(
    function () {
      $(this)
        .find("img")
        .attr(
          "src",
          $(this).find("img").attr("src").replace(".svg", "_on.svg")
        );
    },
    function () {
      $(this)
        .find("img")
        .attr(
          "src",
          $(this).find("img").attr("src").replace("_on.svg", ".svg")
        );
    }
  );
});
