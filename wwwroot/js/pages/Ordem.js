
$(document).ready(function () {
    //const tabelaOS = $('#tabelaOS').DataTable({
    //    responsive: true,
    //    searching: true,
    //    paging: true,
    //    ordering: true,
    //    info: true,
    //    pageLength: 5,
    //    language: {
    //        url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/pt-BR.json"
    //    },
    //    columns: [
    //        { data: "idOrdemServico" },
    //        { data: "cliente" },
    //        { data: "servico" },
    //        { data: "dataAbertura" },
    //        { data: "previsaoEntrega" },
    //        { data: "status" },
    //        {
    //            data: null,
    //            className: "text-center",
    //            render: function (data) {
    //                return `
    //                    <button class="btn btn-warning btn-sm" onclick="EditarOS(${data.idOrdemServico})">
    //                        <i class="bi bi-pencil-square"></i>
    //                    </button>
    //                    <button class="btn btn-danger btn-sm" onclick="ExcluirOS(${data.idOrdemServico})">
    //                        <i class="bi bi-trash"></i>
    //                    </button>
    //                `;
    //            }
    //        }
    //    ]
    //});

    //ObterOrdens();
});

function ObterOrdens() {

    $.ajax({
        url: '/OrdemServico/ObterTodasOrdens',
        type: 'GET',
        success: function (response) {

            console.log(response)

            tabelaOS.clear();      
            tabelaOS.rows.add(response); 
            tabelaOS.draw();       
        },
        error: function (xhr) {
            console.error(xhr);
            Swal.fire("Erro!", "Não foi possível carregar as OS.", "error");
        }
    });
}

function CriarOrdem() {

    var ordemBody = CarregarJsonOrdem();
}

function CarregarJsonOrdem() {
    let cliente = $('#selectCriarOrdemCliente').val();
    let previsaoEntrega = $('#txtCriarOrdemPrevisaoEntrega').val();
    let dataAbertura = $('#txtCriarOrdemDataAbertura').val();
    let produto = $('#selectCriarOrdemProduto').val();
    let descricao = $('#txtCriarOrdemDescricao').val();

    console.log(cliente, previsaoEntrega, dataAbertura, produto, descricao);
}
