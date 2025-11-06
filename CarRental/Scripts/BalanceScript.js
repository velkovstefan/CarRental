
    $(document).ready(function () {

        $.ajax({
            url: '/api/Balances',
            type: 'GET',
            success: function (data) {
                if (data != null && data.MoneyBalance != null) {
                    $('#account-balance').text(data.MoneyBalance);
                }
               
               
            },
            error: function () {
                console.error('Failed to load followers count');
            }
        });
        });
