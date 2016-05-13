var uri = 'api/Book';

$(document).ready(function () {
    // Send an AJAX request
    $.getJSON(uri)
        .done(function (data) {
            // On success, 'data' contains a list of books.
            $.each(data, function (key, item) {
                // Add a list item for the book.
                $('<li>', { text: formatItem(item) }).appendTo($('#books'));
            });
        });
});

function formatItem(item) {
    return item.name + ' ' + item.publishYear;
}

function find() {
    $('#searchedBooks').empty();
    var publishYear = $('#publishYear').val();
    $.getJSON(uri + '/GetBooksByPublishYear/' + publishYear)
        .done(function (data) {
            $.each(data, function (key, item) {
                // Add a list item for the book.
                $('<li>', { text: formatItem(item) }).appendTo($('#searchedBooks'));
            })
        })
        .fail(function (jqXHR, textStatus, err) {
            $('#book').text('Error: ' + err);
        });
}

function create() {
    var name = $('#bookName').val();
    var publisher = $('#bookPublisher').val();
    var publishYear = $('#bookPublishYear').val();

    var model = { Name: name, Publisher: publisher, PublishYear: publishYear };

    $.ajax({
        method: "POST",
        url: uri,
        data: JSON.stringify(model),
        contentType: "application/json; charset=UTF-8",
        dataType: "json",
        success: function (data) {
            $('#success').text(data);
            location.reload();
        }
    })
}