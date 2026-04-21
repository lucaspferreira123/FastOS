// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    const $sidebar = $('#appSidebar');
    let cleanupTimer = null;

    if ($sidebar.length === 0) {
        return;
    }

    $sidebar.on('show.bs.offcanvas', function () {
        if (cleanupTimer) {
            window.clearTimeout(cleanupTimer);
            cleanupTimer = null;
        }

        $('.offcanvas-backdrop').not('.show').remove();
    });

    $sidebar.on('hidden.bs.offcanvas', function () {
        cleanupTimer = window.setTimeout(function () {
            if ($sidebar.hasClass('show')) {
                return;
            }

            $('.offcanvas-backdrop').remove();
            $('body').css({
                overflow: '',
                'padding-right': ''
            });
            cleanupTimer = null;
        }, 200);
    });
});
