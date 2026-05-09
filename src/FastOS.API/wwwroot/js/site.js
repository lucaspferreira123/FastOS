// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(function () {
    const $sidebar = $('#appSidebar');
    let cleanupTimer = null;
    const limparBackdropsOrfaos = function () {
        const existeModalAberto = $('.modal.show').length > 0;
        const existeOffcanvasAberto = $('.offcanvas.show').length > 0;

        if (!existeModalAberto) {
            $('.modal-backdrop').remove();
        }

        if (!existeOffcanvasAberto) {
            $('.offcanvas-backdrop').remove();
        }

        if (!existeModalAberto && !existeOffcanvasAberto) {
            $('body').removeClass('modal-open');
            $('body').css({
                overflow: '',
                'padding-right': ''
            });
        }
    };

    $(document).on('show.bs.modal', '.modal', function () {
        $('.modal-backdrop').not('.show').remove();
    });

    $(document).on('hidden.bs.modal', '.modal', function () {
        window.setTimeout(limparBackdropsOrfaos, 200);
    });

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

            limparBackdropsOrfaos();
            cleanupTimer = null;
        }, 200);
    });
});
