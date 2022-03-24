
(function () {
    $(document).ready(function() {
        $(".dropbtn").click(function(){
            var tgt = $(this).eq(0).attr("id");
            $("[ctntId='" + tgt + "'].dropdown-content").toggleClass("show");

        })
    })
}) ();