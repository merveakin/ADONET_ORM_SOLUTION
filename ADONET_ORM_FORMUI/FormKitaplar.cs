using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ADONET_ORM_BLL;
using ADONET_ORM_Entities;
using ADONET_ORM_Entities.Entities;
using ADONET_ORM_Entities.ViewModels;

namespace ADONET_ORM_FORMUI
{
    public partial class FormKitaplar : Form
    {
        public FormKitaplar()
        {
            InitializeComponent();
        }

        //GLOBAL ALAN
        YazarlarORM myYazarORM = new YazarlarORM();
        TurlerORM myTurlerORM = new TurlerORM();
        KitaplarORM myKitapORM = new KitaplarORM();

        private void FormKitaplar_Load(object sender, EventArgs e)
        {
            TumYazarlariComboyaGetir();
            TumTurleriComboyaGetir();
            TumKitaplariGrideViewModelleGetir();

            TumKitaplariSilComboyaGetir();
            TumKitaplariGuncelleComboyaGetir();

        }

        private void TumKitaplariGuncelleComboyaGetir()
        {
            comboBoxKitapGuncelle.DisplayMember = "KitapAd";
            comboBoxKitapGuncelle.ValueMember = "KitapId";
            comboBoxKitapGuncelle.DataSource = myKitapORM.Select();

        }

        private void TumKitaplariSilComboyaGetir()
        {
            cmbBox_Sil_Kitap.DisplayMember = "KitapAd";
            cmbBox_Sil_Kitap.ValueMember = "KitapId";

            //cmbBox_Sil_Kitap.DataSource =myKitapORM.Select();
            //Yukarıdaki gibi yapmak istemezsek yani
            // KitaplarORM class'ından instance almak istemezsek 
            // class içine tanımladığımız static property aracılığıyla o instance'a ulaşmış oluruz
            // aslında burada kendimize arka planda instance oluşturuyoruz ve static nesne aracılığıyla o nesneyi kullanıyoruz.
            cmbBox_Sil_Kitap.DataSource = KitaplarORM.Current.Select();

        }

        private void TumKitaplariGrideViewModelleGetir()
        {
            dataGridViewKitaplar.DataSource = myKitapORM.KitaplariViewModelleGetir();

            dataGridViewKitaplar.Columns["SilindiMi"].Visible = false;
            dataGridViewKitaplar.Columns["TurId"].Visible = false;
            dataGridViewKitaplar.Columns["YazarId"].Visible = false;
            for (int i = 0; i < dataGridViewKitaplar.Columns.Count; i++)
            {
                dataGridViewKitaplar.Columns[i].Width = 120;
            }
        }
        private void TumTurleriComboyaGetir()
        {
            cmbBox_Ekle_Tur.DisplayMember = "TurAdi";
            cmbBox_Ekle_Tur.ValueMember = "TurId";
            cmbBox_Ekle_Tur.DataSource = myTurlerORM.TurleriGetir();
            cmbBox_Ekle_Tur.SelectedIndex = 0;

            cmbBox_Guncelle_Tur.DisplayMember = "TurAdi";
            cmbBox_Guncelle_Tur.ValueMember = "TurId";
            cmbBox_Guncelle_Tur.DataSource = myTurlerORM.TurleriGetir();
        }

        private void TumYazarlariComboyaGetir()
        {
            cmbBox_Ekle_Yazar.DisplayMember = "YazarAdSoyad";
            cmbBox_Ekle_Yazar.ValueMember = "YazarId";
            cmbBox_Ekle_Yazar.DataSource = myYazarORM.Select();

            cmbBox_Guncelle_Yazar.DisplayMember = "YazarAdSoyad";
            cmbBox_Guncelle_Yazar.ValueMember = "YazarId";
            cmbBox_Guncelle_Yazar.DataSource = myYazarORM.Select();

        }

        private void btnKitapEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericUpDown_Ekle_SayfaSayisi.Value <= 0)
                {
                    MessageBox.Show("HATA : Sayfa sayısı 0'dan büyük olmalıdır!");
                    return;
                }

                if (numericUpDown_Ekle_Stok.Value <= 0)
                {
                    MessageBox.Show("HATA : Stok sayısı 0'dan büyük olmalıdır!");
                    return;
                }

                if ((int)cmbBox_Ekle_Yazar.SelectedValue <= 0)
                {
                    MessageBox.Show("HATA : Kitabın bir yazarı olmalıdır. Yazar seçiniz!");
                    return;
                }

                Kitap yeniKitap = new Kitap()
                {
                    KayitTarihi = DateTime.Now,
                    KitapAd = txtKitapEkle.Text.Trim(),
                    SayfaSayisi = (int)numericUpDown_Ekle_SayfaSayisi.Value,
                    Stok = (int)numericUpDown_Ekle_Stok.Value,
                    SilindiMi = false,
                    YazarId = (int)cmbBox_Ekle_Yazar.SelectedValue
                };

                //TurId null mı değil mi??
                if ((int)cmbBox_Ekle_Tur.SelectedValue == -1)     //Türü Yok seçilmiş demektir.
                {
                    yeniKitap.TurId = null;
                }

                else
                {
                    yeniKitap.TurId = (int)cmbBox_Ekle_Tur.SelectedValue;
                }

                if (myKitapORM.Insert(yeniKitap))
                {
                    MessageBox.Show($"{yeniKitap.KitapAd} başarılı şekilde sisteme eklendi...");
                    TumKitaplariGrideViewModelleGetir();
                    //temizlik
                    EkleSayfasiKontrolleriTemizle();
                    
                    //combobox güncelle ve combobox sil içine buradan yeni eklenen kitaplar da gelmelidir.

                    TumKitaplariGuncelleComboyaGetir();
                    TumKitaplariSilComboyaGetir();
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show("HATA : " + ex.Message);
            }
        }

        private void EkleSayfasiKontrolleriTemizle()
        {
            txtKitapEkle.Clear();
            cmbBox_Ekle_Yazar.SelectedIndex = -1;
            cmbBox_Ekle_Tur.SelectedIndex = -1;
            numericUpDown_Ekle_SayfaSayisi.Value = 0;
            numericUpDown_Ekle_Stok.Value = 0;
        }

        private void btnKitapSil_Click(object sender, EventArgs e)
        {
            try
            {
                if ((int)cmbBox_Sil_Kitap.SelectedValue <= 0)
                {
                    MessageBox.Show("Lütfen kitap seçimi yapınız!", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }

                Kitap kitabim = myKitapORM.SelectET((int)cmbBox_Sil_Kitap.SelectedValue);

                DialogResult cevap = MessageBox.Show($"Bu kitabı silmek yerine pasifleştirmek ister misiniz? \n Pasifleştirmek için ---> Evet \n Silmek için ---> Hayır 'a tıklayınız.", "SİLME ONAY", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (cevap == DialogResult.Yes)
                {
                    //PASİFLEŞTİR >>>>>> UPDATE İLE YAPILMALI...
                    kitabim.SilindiMi = true;
                    switch (myKitapORM.Update(kitabim))
                    {
                        case true:
                            MessageBox.Show($"{kitabim.KitapAd} sistemde pasifleştirildi!");
                            //temizlik
                            SilmeSayfasiKontrolleriTemizle();
                            TumKitaplariSilComboyaGetir();
                            break;

                        case false:
                            throw new Exception($"HATA : {kitabim.KitapAd} pasifleştirme işleminde beklenmedik bir hata oldu.");
                            break;
                    }


                }

                else if (cevap == DialogResult.No)
                {
                    //SİL

                    //linq yazdık.
                    var oduncListe = OduncIslemlerORM.Current.Select().Where(x => x.KitapId == kitabim.KitapId).ToList();
                    if (oduncListe.Count > 0)
                    {
                        MessageBox.Show("DİKKAT : Bu kitap ödünç alınmıştır. Silemezsiniz!!! Silmek isterseniz ödünç işlemler formuna gidip oradan ödünç alma işlem kaydınızı silmeniz gerekmektedir...", "BİLGİ", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }
                    //Yukarıdaki if'e girmezse return olmaz
                    //return olmazsa kod aşağıya doğru okunmaya devam eder...


                    if (myKitapORM.Delete(kitabim))
                    {
                        MessageBox.Show($"{kitabim.KitapAd} adlı kitap silinmiştir.");
                        //temizlik
                        SilmeSayfasiKontrolleriTemizle();
                        TumKitaplariSilComboyaGetir();
                        //kitap silindikten sonra diğer listelerden de kalkmalı.
                        TumKitaplariGuncelleComboyaGetir();
                        TumKitaplariGrideViewModelleGetir();
                    }
                    else
                    {
                        throw new Exception($"HATA : {kitabim.KitapAd} silinememiştir!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("HATA : Silme işleminde beklenmedik bir hata oluştu!" + ex.ToString());
            }
        }

        private void SilmeSayfasiKontrolleriTemizle()
        {
            cmbBox_Sil_Kitap.SelectedIndex = -1;
            richTextBoxKitap.Clear();
        }

        private void comboBoxKitapGuncelle_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GuncelleSayfasiTemizle();
                if (comboBoxKitapGuncelle.SelectedIndex >= 0)
                {
                    Kitap secilenKitap = myKitapORM.SelectET((int)comboBoxKitapGuncelle.SelectedValue);
                    txt_GuncelleKitapAdi.Text = secilenKitap.KitapAd;
                    numericUpDown_Guncelle_SayfaSayisi.Value = secilenKitap.SayfaSayisi;
                    numericUpDown_Guncelle_Stok.Value = secilenKitap.Stok;
                    cmbBox_Guncelle_Yazar.SelectedValue = secilenKitap.YazarId;

                    if (secilenKitap.TurId == null)
                    {
                        //cmbBox_Guncelle_Tur.SelectedIndex = 0;
                        //cmbBox_Guncelle_Tur.SelectedIndex = -1;

                        //cmbBox_Guncelle_Tur.SelectedIndex = 0;
                        //cmbBox_Guncelle_Tur.SelectedValue = -1;
                        // programda belirli değerler varsa örneğin Türü Yok -1 value'ya sahip bir Tur. Böyle durumlarda sabit olan o değerin başka yerde de kullanılması gerekebilir ya da daha sonra o değer değişebilir diye, değeri elle yazmak yerine static bir nesneyle taşıyorlar
                        //  örneğin, Türü Yok -1 değil artık -3 olması gerekliyse
                        // Biz sadece onu ProgramBilgileri class'ında güncelleriz. Böylece onu kullanan ne kadar yer varsa otomatik güncellenmiş olur.
                        cmbBox_Guncelle_Tur.SelectedValue = Sabitler.TurYokSelectedValue;
                    }

                    else
                    {
                        cmbBox_Guncelle_Tur.SelectedValue = secilenKitap.TurId;
                    }


                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("HATA : " + ex.Message);
            }
        }

        private void GuncelleSayfasiTemizle()
        {
            txt_GuncelleKitapAdi.Text = string.Empty;
            numericUpDown_Guncelle_SayfaSayisi.Value = 0;
            numericUpDown_Guncelle_Stok.Value =0;
            cmbBox_Guncelle_Tur.SelectedIndex = -1;
            cmbBox_Guncelle_Yazar.SelectedIndex = -1;
            cmbBox_Guncelle_Yazar.SelectedIndex = -1;

        }

        private void btnKitapGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBoxKitapGuncelle.SelectedIndex >= 0)
                {
                    if (numericUpDown_Guncelle_SayfaSayisi.Value <= 0)
                    {
                        throw new Exception("HATA : Sayfa asyısı sıfırdan büyük olmalıdır!!!");
                    }
                    if (numericUpDown_Guncelle_Stok.Value <= 0)
                    {
                        throw new Exception("HATA : Kitap stoğu sıfırdan büyük olmalıdır!!!");
                    }



                    Kitap secilenKitap = myKitapORM.SelectET((int)comboBoxKitapGuncelle.SelectedValue);
                    if (secilenKitap == null)
                    {
                        //1.YOL
                        throw new Exception("HATA : Kİtap bulunamadı!!!");

                        //2.YOL
                        //MessageBox.Show("HATA : Kitap bulunamadı!!!");
                        //return;
                    }

                    //null değilse zaten buradan devam edecek else gerek kalmadı...

                    secilenKitap.KitapAd = txt_GuncelleKitapAdi.Text.Trim();
                    secilenKitap.SayfaSayisi = (int)numericUpDown_Guncelle_SayfaSayisi.Value;
                    secilenKitap.Stok = (int)numericUpDown_Guncelle_Stok.Value;
                    secilenKitap.SilindiMi = false;
                    secilenKitap.YazarId = (int)cmbBox_Guncelle_Yazar.SelectedValue;

                    if ((int)cmbBox_Guncelle_Tur.SelectedIndex == -1)    //türü yok
                    {
                        secilenKitap.TurId = null;
                    }

                    else
                    {
                        secilenKitap.TurId = (int)cmbBox_Guncelle_Tur.SelectedValue;
                    }

                    switch (myKitapORM.Update(secilenKitap))
                    {

                        case true:
                            MessageBox.Show($"{secilenKitap.KitapAd} başarıyla güncellendi!");
                            //temizlik...
                            //comboBoxKitapGuncelle.SelectedIndex = -1;
                            TumKitaplariGuncelleComboyaGetir();
                            TumKitaplariGrideViewModelleGetir();
                            TumKitaplariSilComboyaGetir();
                            break;

                        case false:
                            throw new Exception($"HATA : {secilenKitap.KitapAd} güncellenirken bir hata oluştu!");
                            
                    }

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("HATA : " + ex.Message);
            }
        }
        private void cmbBox_Sil_Kitap_SelectedIndexChanged(object sender, EventArgs e)
        {
            //richtextbox dolsun.
            if (cmbBox_Sil_Kitap.SelectedIndex >= 0)
            {
                Kitap secilenKitap = myKitapORM.SelectET((int)cmbBox_Sil_Kitap.SelectedValue);
                if (secilenKitap != null)
                {
                    //1.YÖNTEM
                    string turu = secilenKitap.TurId == null ? "Türü yok" :
                       myTurlerORM.Select().FirstOrDefault(x => x.TurId == secilenKitap.TurId)?.TurAdi;

                    richTextBoxKitap.Text = $"Kitap: {secilenKitap.KitapAd} \n" +
                     $"Türü: {turu} \n" +
                        $"Yazar: {myYazarORM.Select().FirstOrDefault(x => x.YazarId == secilenKitap.YazarId).YazarAdSoyad} \n" +
                        $"Sayfa Sayısı: {secilenKitap.SayfaSayisi} \n" +
                        $"Stok: {secilenKitap.Stok} adet stokta var \n";

                    //2 YÖNTEM
                    //if (cmbBox_Sil_Kitap.SelectedIndex >= 0)
                    //{
                    //    KitapViewModel seciliKitap = myKitapORM.KitaplariViewModelleGetir().FirstOrDefault(x => x.KitapId == (int)cmbBox_Sil_Kitap.SelectedValue);
                    //    if (seciliKitap != null)
                    //    {
                    //        richTextBoxKitap.Clear();
                    //        richTextBoxKitap.Text = "Kitap: " + seciliKitap.KitapAd
                    //            + "\nTür:" + seciliKitap.TurAdi
                    //            + "\nYazar: " + seciliKitap.YazarAdSoyad
                    //            + "\n Sayfa Sayısı:" + seciliKitap.SayfaSayisi
                    //            + "\n Stok: " + seciliKitap.Stok + " adet bulunmaktadır";
                    //    }
                    //}


                }
            }
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            //tab'lar değiştikçe temizlik yapılsın.
            EkleSayfasiKontrolleriTemizle();
            comboBoxKitapGuncelle.SelectedIndex = -1;
            GuncelleSayfasiTemizle();
            SilmeSayfasiKontrolleriTemizle();

            
        }
    }
}
