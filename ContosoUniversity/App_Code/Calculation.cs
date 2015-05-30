using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System;
using System.Web.Mvc;


namespace ContosoUniversity
{
    public partial class Calculation
    {
        private Tender tender;

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
                _minPropValue[prop.id] = Bid.QueryAll().Where(b => b.tenderId == tender.id && b.propertyId == prop.id).Select(b => b.defaultValue).Min();
            return _minPropValue[prop.id].Value;
        }

        private Dictionary<int, double?> _maxPropValue = new Dictionary<int, double?>();
        private double maxPropValue(Property prop)
        {
            if (!_maxPropValue.Keys.Contains(prop.id))
                _maxPropValue[prop.id] = Bid.QueryAll().Where(b => b.tenderId == tender.id && b.propertyId == prop.id).Select(b => b.defaultValue).Max();
            return _maxPropValue[prop.id].Value;
        }

        public Calculation(Tender _tender)
        {
            tender = _tender;
            calcFinalScore();
        }


        private void resetData()
        {
            _nonpriceResults = null;
            _priceResults = null;
            _minPropValue = new Dictionary<int, double?>();
            _maxPropValue = new Dictionary<int, double?>();
        }

        private Dictionary<int, double> calcFinalScore()
        {
            resetData();
            Dictionary<int, double> finalScore =new Dictionary<int, double>();
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

        private void calcOptimalPrice()
        {
            
        }



    }
}