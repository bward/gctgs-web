$(".request").each(function() {
    $(this).click(function() {
        var id = $(this).data("board-game-id");

        $.ajax({
            url: '/BoardGames/Request/' + id,
            type: 'POST',
            data: {},
            contentType: 'application/json; charset=utf-8',
            dataType: 'html',
            success: function (data) {
                $("#notifications")
                    .append($(
                        '<div class="alert alert-success fade in out"><a href="#" class="close" data-dismiss="alert">&times;</a><strong>Hooray! </strong>Email request sent successfully)</div>'
                    ).hide().fadeIn(500));
            }
        });
    });
});