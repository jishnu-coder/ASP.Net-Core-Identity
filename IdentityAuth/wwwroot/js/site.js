// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function confirmDelete(userId, isClicked) {
    var deletespan = "delete_" + userId;
    var confirmspan = "ConfirmDelete_" + userId;

    if (isClicked) {
        $("#" + deletespan).hide();
        $("#" + confirmspan).show();
    }
    else {
        $("#" + deletespan).show();
        $("#" + confirmspan).hide();
    }
}