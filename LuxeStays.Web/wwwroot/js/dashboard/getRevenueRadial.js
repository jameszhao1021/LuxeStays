$(document).ready(function () {
    
    loadTotalRevenueRadialChart();
})

function loadTotalRevenueRadialChart() {
    $(".chart-spinner").show();
    $.ajax({
        url: "/Dashboard/GetRevenueRadialChartData",
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            document.querySelector('#spanRevenue').innerHTML = data.totalCount;
            var sectionCurrentCount = document.createElement("span");
            if (data.hasRatioIncrease) {
                sectionCurrentCount.className = "text-success me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-up-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '<span>';
            } else {
                sectionCurrentCount.className = "text-danger me-1";
                sectionCurrentCount.innerHTML = '<i class="bi bi-arrow-down-right-circle me-1"></i> <span>' + data.countInCurrentMonth + '<span>';
            }
            document.querySelector('#sectionRevenue').append(sectionCurrentCount);
            document.querySelector('#sectionRevenue').append("since last month");

            loadRadialBarChart("revenueRadialChart", data);

            $(".chart-spinner").hide();
        }
    })
}