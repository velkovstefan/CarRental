$(document).ready(function () {
    var table = $(".cars").DataTable({
        //scrollX: true,
        responsive: true, 
        columnDefs: [{
            targets: 1,
            render: function (data, type, row) {
               
                if (type === 'filter' || type === 'sort') {

                    return $(data).find(".CarName").text().trim();
                }
                return data;
            }
        }]
        
    });




    async function getUnavalibleDates(carId) {
        var dates = [];
        await $.ajax({
            url: "/Cars/CarDates",
            method: "POST",
            data: {
                carId: carId
            },
            success: function (result) {
                console.log(result);
                var fromDate = new Date();
                dates = result;
            },
            error: function () {
                return [];
            }
        });

        return dates;
    }
    function formatDateDMY(date) {
        var d = date.getDate().toString().padStart(2, "0");
        var m = (date.getMonth() + 1).toString().padStart(2, "0"); 
        var y = date.getFullYear();
        return `${d}/${m}/${y}`;
    }
    function parseDate(str) {
        var parts = str.split("/");
        return new Date(parts[2], parts[1] - 1, parts[0]);
    }
    function setMaxMinData(selectedDate, disabledDates, type) {

        var fromDate = new Date(selectedDate);
        var parsed = disabledDates.map(parseDate);
        console.log(parsed);
        var nextDate = new Date(selectedDate);

        console.log(parsed);
        if (type == 0) {
            var future = parsed.filter(d => d > fromDate);
        }
        else {
            var future = parsed.filter(d => d < fromDate);
        }

        console.log(future);

        if (future.length != 0) {
            if (type == 0) {
                nextDate = new Date(Math.min.apply(null, future));
            }
            else {
                nextDate = new Date(Math.max.apply(null, future));
            }

        }
        else {
            return null;
        }
        console.log(nextDate);
        console.log(formatDateDMY(nextDate));

        return formatDateDMY(nextDate);
    }

    function resetDatepicker(dp) {
        $(dp).val('').datepicker('option', { minDate: 0, maxDate: null });
    }

    function repositionVisibleDatepicker($input) {
        var dp = $(".ui-datepicker:visible");
        if (!dp.length) return;
        var el = $input[0];
        if (!el) return;
        var rect = el.getBoundingClientRect();
        dp.css({
            position: 'fixed',
            top: (rect.bottom) + 'px',
            left: (rect.left) + 'px'
        });
    }
 

    $(document).on("click", ".rentButton", function () {
        var button = $(this);
        console.log($(button).val());





        let promise = getUnavalibleDates($(button).val())
        promise.then((val) => {

            $content = $("<div><h4>Rent this car?</h4><br /> <label for='startDate'>Start Date:</label> <input id='startDate' type='text' class='form-control' /> <label for='endDate'>End Date:</label> <input id='endDate' type='text' class='form-control' /> <button class=\"resetDate form-control\">Reset Date</button></div>");
            console.log(val);

            var $from = $content.find("#startDate");
            var $to = $content.find("#endDate");

            var disabledDates = val;

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate("dd/mm/yy", element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }

            
            function fixedBeforeShow(inputElem, inst) {
                
                setTimeout(function () {
                    var rect = inputElem.getBoundingClientRect();
                    inst.dpDiv.css({
                        position: 'fixed',
                        top: (rect.bottom) + 'px',
                        left: (rect.left) + 'px',
                        zIndex: 3000
                    });
                }, 0);

            }
            $from.datepicker({
                //defaultDate: "+1w",
                dateFormat: "dd/mm/yy",  
                changeMonth: true,
                numberOfMonths: 1,
                minDate: 0,
                appendTo: $from.parent(), 
                beforeShow: fixedBeforeShow,
                beforeShowDay: function (date) {
                    var string = $.datepicker.formatDate("dd/mm/yy", date);
                    return [disabledDates.indexOf(string) === -1];
                }
            })
                .on("change", function () {
                    var selectedDate = getDate(this);
                    if (selectedDate) {
                        $to.datepicker("option", "minDate", selectedDate);

                        let nextDate = setMaxMinData(selectedDate, disabledDates, 0);

                        if (nextDate != null) {
                            $to.datepicker("option", "maxDate", nextDate);
                        }
                    }
                });

            //var $to = $content.find("#endDate");

            $to.datepicker({
                //defaultDate: "+1w",
                dateFormat: "dd/mm/yy",   
                changeMonth: true,
                numberOfMonths: 1,
                minDate: 0,
                appendTo: $to.parent(),
                beforeShow: fixedBeforeShow,
                beforeShowDay: function (date) {
                    var string = jQuery.datepicker.formatDate("dd/mm/yy", date);
                    return [disabledDates.indexOf(string) == -1]
                }
            }).on("change", function () {

                var selectedDate = getDate(this);
                if (selectedDate) {
                    $from.datepicker("option", "maxDate", selectedDate);
                    var selectedDateFrom = getDate($from[0]);
                    let nextDate;
                    console.log(this);
                    console.log($from[0]);

                    if (selectedDateFrom == null) {
                        nextDate = setMaxMinData(selectedDate, disabledDates, 1);
                    }
                    else {
                        nextDate = 0;
                    }


                    if (nextDate != null) {
                        $from.datepicker("option", "minDate", nextDate);
                    }

                }
            });



            bootbox.confirm({
                message: $content,
                buttons: {
                    cancel: {
                        label: '<i class="fa fa-times"></i> Cancel'
                    },
                    confirm: {
                        label: '<i class="fa fa-check"></i> Confirm'
                    }
                },
                callback: function (result) {
                    if (result) {
                        $.ajax({
                            url: "/Cars/RentCar",
                            method: "POST",
                            data: {
                                carId: $(button).val(),
                                start: $("#startDate").val(),
                                end: $("#endDate").val()
                            },
                            success: function (result) {
                                console.log(result);
                                showMessage(result.message);
                            },
                            error: function () {

                            }

                        })
                    }
                },
            })
            $(document).on("click", ".resetDate", function () {
                resetDatepicker($from);
                resetDatepicker($to);

            })

        })





    });

    $(document).on("click", ".serviceButton", function () {
        var button = $(this);
        console.log($(button).val());
        bootbox.confirm({
            message: "<h4>Chose type of service</h4><label for='typeOfService'>Type of service:</label> <select id='typeOfService'><option value='minor'>Minor</option><option value='major'>Major</option></select>",
            buttons: {
                cancel: {
                    label: '<i class="fa fa-times"></i> Cancel'
                },
                confirm: {
                    label: '<i class="fa fa-check"></i> Confirm'
                }
            },
            callback: function (result) {
                console.log(result);

                if (result) {

                    $.ajax({
                        url: "/Cars/ServiceCar",
                        method: "POST",
                        data: {
                            carId: $(button).val(),
                            typeOfService: $("#typeOfService").val()

                        },
                        success: function (result) {
                            console.log(result);
                            showMessage(result.message);
                        },
                        error: function () {

                        },
                        finally: function () {

                        }

                    })
                }

            },
        })
    })
    function showMessage(message) {
        bootbox.alert({
            message: message,

        });
    }

    $(document).on("click", ".delete", function () {
        var button = $(this);
        console.log($(button).val())
        bootbox.confirm({
            message: "Are you sure you want to delete this car?", buttons: {
                cancel: {
                    label: 'No'
                },
                confirm: {
                    label: 'Yes'
                }
            }, callback: function (result) {
                if (result) {

                    $.ajax({
                        url: "/Cars/DeleteConfirmed",
                        method: "POST",
                        data: {
                            id: $(button).val(),

                        },
                        success: function () {

                            $(button).parents("tr").remove();
                        },
                        error: function () {
                            console.log($(button).val())
                        }

                    })

                }
            }
        })

    });




});