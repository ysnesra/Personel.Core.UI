/*site.js içine yazmamızın sebebi her yerde kullanabilmek için
Departman silerken "emin misiniz" sorusunu sordurmak için */

function CheckDateTypeIsValid(dateElement) {     //fonksiyon oluşturulmuş
    var value = $(dateElement).val(); 
    if (value == '') {
        $(dateElement).valid("false")
    }
    else {
        $(dateElement).valid();
    }
}


$(function () {
    $("#tblDepartmanlar").on("click", ".tblDepartmanSil", function () {  
        var btn = $(this);                    //btn değişkeninde bu butonu saklayalım.
      
        bootbox.confirm("Departmanı silmek istediğinize emin misiniz?", function (result) { //alert yerine confirm ile evet hayırı sorguluyoruz."bootbox.confirm" alert deki gibi pencere açılmasını sağlar.
            if (result) {
                var id = btn.data("id");
                $.ajax({
                    type: "GET",
                    url: "/Departman/Delete/" + id,
                    success: function () {
                        btn.parent().parent().remove();                   //iki kez parentını alıyoruz.
                    }
                });

            }
        });
    });
});


