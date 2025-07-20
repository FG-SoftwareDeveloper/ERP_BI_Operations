let isCardView = false;

$(document).ready(function () {
    // Initialize global DataTable loader
    LoadDataTable({
        tableId: "accountsTable",
        ajaxUrl: "/api/Accounts", // Adjust based on your API
        columns: [
            { data: "accountId", title: "Account ID" },
            { data: "accountName", title: "Account Name" },
            { data: "accountType", title: "Account Type" },
            { data: "balance", title: "Balance" },
            { data: "status", title: "Status" }
        ]
    });

    // Toggle button click handler
    $('#toggleViewBtn').on('click', function () {
        isCardView = !isCardView;
        const $btn = $(this);
        const $table = $('#accountsTable');

        if (isCardView) {
            $btn.text("Switch to Table View");
            $table.addClass("cards");
        } else {
            $btn.text("Switch to Card View");
            $table.removeClass("cards");
        }

        $table.DataTable().draw(); // Redraw to apply view
    });
});

function LoadDataTable(config) {
    if (!config || !config.tableId || !config.columns || !config.ajaxUrl) {
        console.error("Invalid DataTable config.");
        return;
    }

    const $table = $(`#${config.tableId}`);

    $table.DataTable({
        destroy: true,
        processing: true,
        serverSide: false,
        ajax: {
            url: config.ajaxUrl,
            method: config.ajaxMethod || 'GET',
            dataSrc: config.dataSrc || ''
        },
        columns: config.columns,
        pageLength: config.pageLength || 10,
        responsive: true,
        order: config.defaultOrder || [],
        drawCallback: function () {
            const api = this.api();

            if (isCardView) {
                api.rows().every(function () {
                    const $row = $(this.node());
                    $row.addClass("d-block");
                });
            } else {
                api.rows().every(function () {
                    const $row = $(this.node());
                    $row.removeClass("d-block");
                });
            }
        }
    });

    console.log(`DataTable [${config.tableId}] initialized.`);
}
