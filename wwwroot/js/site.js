// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FecharOffcanvasEBackdrop() {

    // 1. forço a remoção do backdrop (Solução de força bruta)
    setTimeout(function () {
        $('.offcanvas-backdrop').remove();
        $('body').removeClass('offcanvas-open');
    }, 300); // Um pequeno atraso para dar tempo ao Bootstrap de começar a fechar
}