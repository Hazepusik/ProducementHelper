using System.Collections.Generic;
using System.Linq;
using System;
using System.Diagnostics;
using System.IO;

using System.Data;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System.Web;


namespace ContosoUniversity
{
    public partial class Calculation
    {
        private Tender tender;

        private const double priceEps = 10;

        private const int stepCnt = 10;

        public static int StepCnt()
        {
            return stepCnt;
        }

        private const int experimentCnt = 100;

        private int oursParticipantId
        {
            get { return participants.First(p => p.isOurs ?? false).id; }
        }

        private Property _priceProperty = null;
        private Property priceProperty
        {
            get 
            {
                if (_priceProperty == null)
                    _priceProperty = Property.QueryAll().First(p => p.isPrice == true && tender.propertyIds.Contains(p.id));
                return _priceProperty; 
            }
        }

        private Dictionary<int, Dictionary<int, double>> _nonpriceResults = null;
        private double getNonPriceResult(Participant Participant, Property Property)
        {
            if (_nonpriceResults == null)
            {
                _nonpriceResults = new Dictionary<int, Dictionary<int, double>>();
                foreach (Participant part in participants)
                {
                    Dictionary<int, double> partResults = new Dictionary<int, double>();
                    foreach (Property prop in properties.Where(p => !(p.isPrice ?? false)))
                    {
                        if (!prop.toMax) // iii 21, 22
                        {
                            double min = Math.Max(minPropValue(prop), prop.minValue ?? -10e+100);
                            partResults[prop.id] = min / getBidValue(part, prop) * 100;
                        }
                        else
                        {
                            double max = Math.Min(maxPropValue(prop), prop.maxValue ?? 10e+100);
                            partResults[prop.id] = getBidValue(part, prop) / max * 100;
                        }
                    }
                    _nonpriceResults[part.id] = partResults;
                }
            }
            return _nonpriceResults[Participant.id][Property.id];
        }


        Dictionary<int, double> _priceResults = null;
        private double getPriceResult(Participant Participant)
        {
            if (_priceResults == null)
            {
                _priceResults = new Dictionary<int, double>();
                foreach (Participant p in participants)
                {
                    if (minPropValue(priceProperty) > 0)
                    {
                        _priceResults[p.id] = minPropValue(priceProperty) / getBidValue(p, priceProperty) * 100;
                    }
                    else
                    {
                        _priceResults[p.id] = (maxPropValue(priceProperty) - getBidValue(p, priceProperty)) / maxPropValue(priceProperty) * 100;
                    }
                }
            }
            return _priceResults[Participant.id];
        }

        Dictionary<int, Dictionary<int, double>> _bidValues = null;
        private double getBidValue(Participant Participant, Property Property)
        {
            if (_bidValues == null)
            {
                _bidValues = new Dictionary<int, Dictionary<int, double>>();
                foreach (Participant pt in participants)
                {
                    Dictionary<int, double> ptValues = new Dictionary<int, double>();
                    foreach (Property py in properties)
                    {
                        ptValues[py.id] = Bid.QueryAll().First(b => b.tenderId == tender.id && b.participantId == pt.id && b.propertyId == py.id).defaultValue;
                    }
                    _bidValues[pt.id] = ptValues;
                }
            }
            return _bidValues[Participant.id][Property.id];
        }

        Dictionary<int, Dictionary<int, double>> _bidMaxValues = null;
        private double getBidMaxValue(Participant Participant, Property Property)
        {
            if (_bidMaxValues == null)
            {
                _bidMaxValues = new Dictionary<int, Dictionary<int, double>>();
                foreach (Participant pt in participants)
                {
                    Dictionary<int, double> ptValues = new Dictionary<int, double>();
                    foreach (Property py in properties)
                    {
                        Bid bid = Bid.QueryAll().First(b => b.tenderId == tender.id && b.participantId == pt.id && b.propertyId == py.id);
                        ptValues[py.id] = (bid.maxValue ?? 0) == 0 ? bid.defaultValue : bid.maxValue.Value;
                    }
                    _bidMaxValues[pt.id] = ptValues;
                }
            }
            return _bidMaxValues[Participant.id][Property.id];
        }

        /// <summary>
        /// Participant, Property, Value, Count
        /// </summary>
        Dictionary<int, Dictionary<int, Dictionary<double, int>>> experiments = null;
        List<int> expMaxCnt = null;
        List<Tuple<int, int>> expPairs = null;

        private double generateValues() //Participant Participant, Property Property)
        {
            if (experiments == null)
            {
                resetValues();
                resetData();
                Random rnd = new Random(new System.DateTime().Millisecond);
                experiments = new Dictionary<int, Dictionary<int, Dictionary<double, int>>>();
                expMaxCnt = new List<int>();
                expPairs = new List<Tuple<int, int>>();
                foreach (Participant pt in participants)
                {
                    Dictionary<int, Dictionary<double, int>> ptValues = new Dictionary<int, Dictionary<double, int>>();
                    foreach (Property py in properties)
                    {
                        Dictionary<double, int> ptPyValues = new Dictionary<double, int>();
                        double min = getBidValue(pt, py);
                        double max = getBidMaxValue(pt, py);
                        double step = (max - min) / stepCnt;
                        int stepsTotal = stepCnt;
                        if ((py.step ?? 0) != 0)
                        {
                            step = py.step.Value;
                            stepsTotal = (int)((max - min) / step);
                        }
                        if (stepsTotal > stepCnt)
                        {
                            step = (max - min) / stepCnt;
                            stepsTotal = stepCnt;
                        }
                        if (min == max)
                            ptPyValues[min] = experimentCnt;
                        else
                            for (int i=0; i < experimentCnt; ++i) // generate new number
                            {
                                if (py.functionId == 1 || true) // TODO : fixit
                                {
                                    double rndVal = min + rnd.Next(0, stepsTotal + 1) * step;
                                    if (!ptPyValues.Keys.Contains(rndVal))
                                        ptPyValues[rndVal] = 0;
                                    ptPyValues[rndVal] += 1;
                                }
                            }
                        ptValues[py.id] = ptPyValues;
                        expMaxCnt.Add(ptPyValues.Keys.Count-1);
                        Tuple<int, int> pair = new Tuple<int, int>(pt.id, py.id);
                        expPairs.Add(pair);
                    }
                    experiments[pt.id] = ptValues;
                }

            }
            return 1; // _experiments[Participant.id][Property.id];
        }

        private List<Participant> _participants = null;
        List<Participant> participants
        {
            get
            {
                if (_participants == null)
                    _participants = Participant.QueryAll().Where(p => tender.participantIds.Contains(p.id)).ToList();
                return _participants;
            }
        }

        private List<Property> _properties = null;
        List<Property> properties
        {
            get
            {
                if (_properties == null)
                    _properties = Property.QueryAll().Where(p => tender.propertyIds.Contains(p.id)).ToList();
                return _properties;
            }
        }

        private Dictionary<int, double?> _minPropValue = new Dictionary<int, double?>();
        private double minPropValue(Property prop)
        {
            if (!_minPropValue.Keys.Contains(prop.id))
            {
                _minPropValue[prop.id] = _bidValues[oursParticipantId][prop.id];  // simply init
                foreach (int partId in participants.Select(p => p.id))
                    _minPropValue[prop.id] = Math.Min(_bidValues[partId][prop.id], _minPropValue[prop.id].Value);
            }
            return _minPropValue[prop.id].Value;
        }

        private Dictionary<int, double?> _maxPropValue = new Dictionary<int, double?>();
        public double priceForBest;
        public double priceForWorst;
        private double maxPropValue(Property prop)
        {
            if (!_maxPropValue.Keys.Contains(prop.id))
            {
                _maxPropValue[prop.id] = _bidValues[oursParticipantId][prop.id];  // simply init
                foreach (int partId in participants.Select(p => p.id))
                    _maxPropValue[prop.id] = Math.Max(_bidValues[partId][prop.id], _maxPropValue[prop.id].Value);
            }
            return _maxPropValue[prop.id].Value;
        }

        public Calculation(Tender _tender)
        {
            tender = _tender;
            //calcOptimalPrice();
            //generateValues();
        }


        private void resetData()
        {
            _nonpriceResults = null;
            _priceResults = null;
            _minPropValue = new Dictionary<int, double?>();
            _maxPropValue = new Dictionary<int, double?>();
        }

        private void resetValues()
        {
            _bidValues = null;
            getBidValue(participants.First(), properties.First());
        }

        private Dictionary<int, double> calcFinalScore()
        {
            resetData();
            Dictionary<int, double> finalScore = new Dictionary<int, double>();
            foreach (Participant part in participants)
            {
                double score = 0;            
                foreach (Property prop in properties.Where(p => !(p.isPrice ?? false)))
                {
                    score += getNonPriceResult(part, prop) * prop.importance / 100;
                }
                finalScore[part.id] = (score * (100 - priceProperty.importance) + getPriceResult(part) * priceProperty.importance) / 100;
            }
            return finalScore;
        }

        void setBid(Participant Participant, Property Property, double price)
        {
            _bidValues[Participant.id][Property.id] = price;
        }

        void setOurBidPrice(double price)
        {
            _bidValues[oursParticipantId][priceProperty.id] = price;
        }
        private bool ourWin(double? ourPrice = null)
        {
            if (ourPrice.HasValue)
                setOurBidPrice(ourPrice.Value);
            Dictionary<int, double> finalScore = calcFinalScore();
            return finalScore.Values.Max() == finalScore[oursParticipantId];
        }

        public double calcOptimalPrice(bool resetVals = true)
        {
            if (resetVals)
                resetValues();
            resetData();
            double a = 0;
            if (!ourWin(a))
                return 0;

            double b = maxPropValue(priceProperty);
            while (ourWin(b))
                b *= 2;

            double x = (a + b) / 2;
            double prevx = x + 2 * priceEps;
            while (Math.Abs(prevx-x) > priceEps)
            {
                if (ourWin(x))
                    a = x;
                else
                    b = x;
                prevx = x;
                x = (a + b) / 2;
            }
            return Math.Round(x - 2 * priceEps, 2);
        }

        private void initFromList(List<int> vals)
        {
            for (int i = 0; i<vals.Count; ++i)
            {
                int ptId = expPairs[i].Item1;
                int pyId = expPairs[i].Item2;
                Dictionary<double, int> ext = experiments[ptId][pyId];
                //double old = _bidValues[ptId][pyId];
                //double ne = ext.Keys.ElementAt(vals[i]);
                if (!properties.First(p => p.id == pyId).toMax)
                    _bidValues[ptId][pyId] = ext.Keys.OrderBy(k => k).ElementAt(0);
                else
                    _bidValues[ptId][pyId] = ext.Keys.OrderBy(k => k).ElementAt(vals[i]);
            }
        }

        public bool sumIs100()
        {
            int ppId = priceProperty.id;
            double imps = properties.Where(p => p.id != ppId).Select(p => p.importance).Sum();
            return imps == 100;
        }

        public List<Dot> generateGraphData()
        {

            List<Dot> result = new List<Dot>();

            int priceSteps = 100;
            resetData();
            generateValues();
            List<int> index = new List<int>(new int[expMaxCnt.Count]);
            double prevPrice = -1;
            priceForBest = calcOptimalPrice();
            initFromList(expMaxCnt);
            bool x = ourWin(10896208);
            priceForWorst = calcOptimalPrice(false);
            if (priceProperty.maxValue.HasValue && !priceProperty.toMax)
            {
                priceForWorst = Math.Min(priceForWorst, (priceProperty.maxValue.Value));
                priceForBest = Math.Min(priceForBest, (priceProperty.maxValue.Value));
            }
            if (Math.Abs(priceForWorst - priceForBest) < priceEps)
            {
                result.Add(new Dot(0, 100));
                result.Add(new Dot(priceForBest, 100));
                return result;
            }
            double step = (priceForBest - priceForWorst) / priceSteps;
            double price = priceForWorst;
            while (price <= priceForBest + step / 2)
            {

                index = new List<int>(new int[expMaxCnt.Count]);
                //initFromList(index);
                index[0] = -1;
                double percent = 0;
                /////////
                //initFromList(expMaxCnt);
                //double newWorstPrice = calcOptimalPrice(false);
                //if (newWorstPrice == prevPrice)
                //{
                //    result.Add(new Dot(price, Math.Round(percent * 100, 2)));
                //    price += step;
                //    continue;
                //}
                //prevPrice = newWorstPrice;
                ////////
                while (!expMaxCnt.SequenceEqual(index))
                {
                    index = iter(index);
                    initFromList(index);
                    if (ourWin(price))
                        percent += calcPercent(index);
                    else
                        percent = percent;
                }
                result.Add(new Dot(price, Math.Round(percent*100,2)));
                price += step;
            }
            result.Add(new Dot(price, 0));
            return result;
        }

        private List<int> iter(List<int> index)
        {
            List<int> result = index;
            result[0] += 1;
            int i = 0;
            while (result[i] > expMaxCnt[i])
            {
                result[i] = 0;
                result[i+1] += 1;
                i++;
            }
            return result;
        }

        private double calcPercent(List<int> vals)
        {
            double result = 1;
            for (int i = 0; i < vals.Count; ++i)
            {
                int ptId = expPairs[i].Item1;
                int pyId = expPairs[i].Item2;
                Dictionary<double, int> ext = experiments[ptId][pyId];
                double eRes = ext[ext.Keys.OrderBy(k => k).ElementAt(vals[i])];
                result *= eRes / experimentCnt;
            }
            return result;
        }

        private static ExcelWorksheet CreateSheet(ExcelPackage p, string sheetName)
        {
            p.Workbook.Worksheets.Add(sheetName);
            ExcelWorksheet ws = p.Workbook.Worksheets[p.Workbook.Worksheets.Count];
            ws.Name = sheetName; //Setting Sheet's name
            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

            return ws;
        }

        public string generateXlsx()
        {
            resetValues();
            using (ExcelPackage package = new ExcelPackage())
            {
                int defaultC = 5;
                int defaultR = 5;
                package.Workbook.Properties.Author = "Khazov Andrey";
                package.Workbook.Properties.Title = "Comparation table";

                //Create a sheet
                ExcelWorksheet ws = CreateSheet(package, "Сравнительные характеристики");

                int c = defaultC;
                int r = defaultR - 2;

                foreach (Participant part in participants)
                {
                    ws.Cells[r, c].Value = part.name;
                    ws.Cells[r, c, r, c + 1].Merge = true;
                    ws.Cells[r + 1, c].Value = "Баллы";
                    ws.Cells[r + 1, c + 1].Value = "Им. ед.";
                    c += 2;
                }
                c = defaultC - 3;
                r += 2;
                ws.Cells[r, c].Value = priceProperty.name;
                ws.Cells[r, c, r, c + 1].Merge = true;
                ws.Cells[r, c + 2].Value = priceProperty.importance.ToString() + "%";
                r++;
                ws.Cells[r, c].Value = "Неценовые";
                ws.Cells[r, c, r, c + 1].Merge = true;
                ws.Cells[r, c + 2].Value = (100 - priceProperty.importance).ToString() + "%";
                r++;
                foreach (Property prop in properties.Where(p => !(p.isPrice ?? false)))
                {
                    ws.Cells[r, c + 1].Value = prop.name;
                    ws.Cells[r, c + 2].Value = prop.importance.ToString() + "%";
                    r++;
                }
                ws.Cells[r, c].Value = "Итого";
                ws.Cells[r, c, r, c + 1].Merge = true;
                ws.Cells[r, c + 2].Value = "100%";
                c = defaultC;
                r = defaultR;
                foreach (Participant part in participants)
                {
                    ws.Cells[r, c].Value = Math.Round(getPriceResult(part), 2);
                    ws.Cells[r, c + 1].Value = getBidValue(part, priceProperty);
                    r += 2;
                    foreach (Property prop in properties.Where(p => !(p.isPrice ?? false)))
                    {
                        ws.Cells[r, c].Value = Math.Round(getNonPriceResult(part, prop), 2);
                        ws.Cells[r, c + 1].Value = getBidValue(part, prop);
                        r += 1;
                    }
                    r = defaultR;
                    c += 2;
                }

                r = defaultR + properties.Count + 1;
                c = defaultC;
                Dictionary<int, double> finalScore = calcFinalScore();
                foreach (Participant part in participants)
                {
                    ws.Cells[r, c].Value = Math.Round(finalScore[part.id], 2);
                    c += 2;
                }

                //Generate A File with Random name
                Byte[] bin = package.GetAsByteArray();
                string path = "Отчет_" + this.tender.name + DateTime.Now.ToString("_HH_mm_ss-dd_MM_yyyy") + ".xlsx";
                string fileName = Path.Combine(HttpRuntime.AppDomainAppPath, "Reports", path);
                File.WriteAllBytes(fileName, bin);

                //These lines will open it in Excel
                //ProcessStartInfo pi = new ProcessStartInfo(fileName);
                //Process.Start(pi);
                return "/Reports/" + path;
            }
        }
    }


    public partial class Dot
    {
        public double price;
        public double percent;
        public Dot(double _price, double _percent)
        {
            this.price = _price;
            this.percent = _percent;
        }
    }
}