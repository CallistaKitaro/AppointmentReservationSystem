// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var d = new Date();
var strDate = (d.getDate()+1) + "/" + (d.getMonth() + 1) + "/" + d.getFullYear();

$('.date.SearchDate').datepicker({
    format: "dd/mm/yyyy",
    startDate: strDate,
    daysOfWeekDisabled: "0,6",
});