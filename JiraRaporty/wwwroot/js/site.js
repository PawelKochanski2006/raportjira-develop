// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    // Initialize Select2 with multiple selection and search
    $('#multiSelect').select2({
        placeholder: "Select options",
        allowClear: true,
        closeOnSelect: false,
        width: '100%'
    });
});