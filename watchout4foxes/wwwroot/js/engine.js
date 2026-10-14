(function () {
    'use strict';

    const Lexicon = {
        ready: false,
        allWords: [],
        buckets: {},
        posBuckets: {},

        posMap: {
            'n': 'Noun',
            'a': 'Adjective',
            't': 'VerbTransitive',
            'i': 'VerbIntransitive',
            'e': 'Adverb',
            'z': 'Interjection',
            's': 'Preposition'
        },

        levelMap: {
            '10': 'VeryCommon',
            '20': 'Common',
            '35': 'SomewhatCommon',
            '50': 'Uncommon',
            '60': 'Uncommon',
            '70': 'Obscure',
            '95': 'Obscure'
        },

        init: function () {
            const self = this;
            return fetch('lexicon.tsv')
                .then(function (res) {
                    if (!res.ok) throw new Error('Failed to fetch lexicon: ' + res.status);
                    return res.text();
                })
                .then(function (text) {
                    const lines = text.split('\n');
                    for (let i = 0; i < lines.length; i++) {
                        const line = lines[i].trim();
                        if (!line || line.startsWith('#')) continue;

                        const parts = line.split('\t');
                        if (parts.length < 3) continue;

                        const word = parts[0].trim();
                        const posRaw = parts[1].trim();
                        const level = parts[2].trim();

                        self.allWords.push(word);

                        const posList = posRaw.split(',');
                        for (let p = 0; p < posList.length; p++) {
                            const pos = posList[p].trim();

                            if (!self.posBuckets[pos]) self.posBuckets[pos] = [];
                            self.posBuckets[pos].push(word);

                            const key = `${pos}_${level}`;
                            if (!self.buckets[key]) self.buckets[key] = [];
                            self.buckets[key].push(word);
                        }
                    }
                    self.ready = true;
                    console.log(`[watchout4foxes] Pristine lexicon loaded: ${self.allWords.length} words in memory.`);
                })
                .catch(function (err) {
                    console.error('[watchout4foxes] Error initializing lexicon:', err);
                });
        },

        getWord: function (posCode, levelCode) {
            if (!this.ready || this.allWords.length === 0) return 'fox';

            const posName = this.posMap[posCode];
            const levelName = this.levelMap[levelCode];

            if (posName && levelName) {
                const bucket = this.buckets[`${posName}_${levelName}`];
                if (bucket && bucket.length > 0) {
                    return bucket[Math.floor(Math.random() * bucket.length)];
                }
            }

            if (posName) {
                const posArr = this.posBuckets[posName];
                if (posArr && posArr.length > 0) {
                    return posArr[Math.floor(Math.random() * posArr.length)];
                }
            }

            return this.allWords[Math.floor(Math.random() * this.allWords.length)];
        },

        generatePhrase: function (slots) {
            const words = [];
            for (let i = 0; i < slots.length; i++) {
                if (!slots[i].pos) continue;
                words.push(this.getWord(slots[i].pos, slots[i].level));
            }
            if (words.length === 0) {
                words.push(this.getWord('a', '35'));
                words.push(this.getWord('n', '35'));
            }
            return words.join(' ');
        },

        conjugateThirdPerson: function (verb) {
            if (!verb) return 'foxes';
            if (verb.endsWith('ing') || verb.endsWith('ed')) return verb;
            if (/([sxz]|sh|ch)$/.test(verb)) return verb + 'es';
            if (/[^aeiou]y$/.test(verb) && verb.length > 1) return verb.slice(0, -1) + 'ies';
            return verb + 's';
        },

        capitalize: function (text) {
            return text ? text.charAt(0).toUpperCase() + text.slice(1) : '';
        },

        getDeterminer: function (followingWord, capitalize) {
            const roll = Math.random();
            let det;

            if (roll < 0.50) {
                det = 'the';
            } else if (roll < 0.75) {
                const firstChar = followingWord ? followingWord.trim().charAt(0).toLowerCase() : 'c';
                det = 'aeiou'.includes(firstChar) ? 'an' : 'a';
            } else if (roll < 0.90) {
                const possessives = ['your', 'his', 'her', 'its', 'my', 'our'];
                det = possessives[Math.floor(Math.random() * possessives.length)];
            } else {
                const rare = ['every', 'another', 'each', 'this'];
                det = rare[Math.floor(Math.random() * rare.length)];
            }

            return capitalize ? det.charAt(0).toUpperCase() + det.slice(1) : det;
        },

        getPreposition: function () {
            const preps = [
                'beside', 'within', 'outside', 'on top of', 'across', 'throughout',
                'beneath', 'underneath', 'opposite', 'near', 'past', 'after', 'before', 'behind'
            ];
            return preps[Math.floor(Math.random() * preps.length)];
        },

        generateSentence: function (customSubject, otherSubject, archetype) {
            const noun1 = this.getWord('n', '20');
            let noun2 = this.getWord('n', '20');
            while (noun2 === noun1) noun2 = this.getWord('n', '20');

            let noun3 = this.getWord('n', '20');
            while (noun3 === noun1 || noun3 === noun2) noun3 = this.getWord('n', '20');

            const adj1 = this.getWord('a', '20');
            let adj2 = this.getWord('a', '20');
            while (adj2 === adj1) adj2 = this.getWord('a', '20');

            const verbT_raw = this.getWord('t', '20');
            const verbI_raw = this.getWord('i', '20');
            const verbT = this.conjugateThirdPerson(verbT_raw);
            const verbI = this.conjugateThirdPerson(verbI_raw);
            const prep = this.getPreposition();

            const s1_cap = customSubject || `${this.getDeterminer(noun1, true)} ${noun1}`;
            const s1_low = customSubject || `${this.getDeterminer(noun1, false)} ${noun1}`;
            const obj = otherSubject || `${this.getDeterminer(adj1, false)} ${adj1} ${noun2}`;

            const prepPhrase_low = `${prep} ${this.getDeterminer(adj2, false)} ${adj2} ${noun3}`;
            const prepPhrase_cap = `${this.capitalize(prep)} ${this.getDeterminer(adj2, false)} ${adj2} ${noun3}`;

            const type = archetype !== undefined ? archetype : Math.floor(Math.random() * 5);

            switch (type) {
                case 0:
                    return `${s1_cap} ${verbT} ${obj} ${prepPhrase_low}.`;

                case 1:
                    return `${s1_cap} ${verbI} ${prepPhrase_low}.`;

                case 2:
                    return `${prepPhrase_cap} ${verbI} ${s1_low}.`;

                case 3: {
                    const modals = ['Why can\'t', 'Why won\'t', 'Will', 'Can', 'How will', 'Should', 'Does'];
                    const modal = modals[Math.floor(Math.random() * modals.length)];
                    const targetVerb = Math.random() < 0.5 ? verbT_raw : verbI_raw;
                    const tail = targetVerb === verbT_raw ? `${targetVerb} ${obj}` : `${targetVerb} ${prepPhrase_low}`;
                    return `${modal} ${s1_low} ${tail}?`;
                }

                default: {
                    const punct = Math.random() < 0.5 ? '!' : '.';
                    return `${s1_cap} ${verbI}${punct}`;
                }
            }
        },

        generateParagraph: function (sub1, sub2) {
            const s1 = sub1 && sub1.trim() ? this.capitalize(sub1.trim()) : null;
            const s2 = sub2 && sub2.trim() ? this.capitalize(sub2.trim()) : null;

            const count = 4 + Math.floor(Math.random() * 2);
            const sentences = [];

            const archetypes = [0, 1, 4, 3, 2];

            for (let i = 0; i < count; i++) {
                const current = (i % 2 === 0) ? s1 : s2;
                const other = (i % 2 === 0) ? s2 : s1;
                sentences.push(this.generateSentence(current, other, archetypes[i % archetypes.length]));
            }

            return sentences.join(' ');
        }
    };

    document.addEventListener('DOMContentLoaded', function () {
        Lexicon.init().then(function () {
            const resultEl = document.getElementById('result');

            const frmPhrase = document.getElementById('frmPhrase');
            if (frmPhrase) {
                frmPhrase.addEventListener('submit', function (e) {
                    e.preventDefault();
                    const slots = [];
                    for (let i = 1; i <= 4; i++) {
                        const posEl = document.getElementById(`Pos${i}`);
                        const lvlEl = document.getElementById(`Level${i}`);
                        if (posEl && posEl.value) {
                            slots.push({ pos: posEl.value, level: lvlEl ? lvlEl.value : '35' });
                        }
                    }
                    if (resultEl) resultEl.textContent = Lexicon.generatePhrase(slots);
                });
            }

            const frmWord = document.getElementById('frmWord');
            if (frmWord) {
                frmWord.addEventListener('submit', function (e) {
                    e.preventDefault();
                    const posEl = document.getElementById('Pos');
                    const lvlEl = document.getElementById('Level');
                    const pos = posEl ? posEl.value : '';
                    const level = lvlEl ? lvlEl.value : '';
                    if (resultEl) resultEl.textContent = Lexicon.getWord(pos, level);
                });
            }

            const frmSentence = document.getElementById('frmSentence');
            if (frmSentence) {
                frmSentence.addEventListener('submit', function (e) {
                    e.preventDefault();
                    if (resultEl) resultEl.textContent = Lexicon.generateSentence();
                });
            }

            const frmParagraph = document.getElementById('frmParagraph');
            if (frmParagraph) {
                frmParagraph.addEventListener('submit', function (e) {
                    e.preventDefault();
                    const s1El = document.getElementById('Subject1');
                    const s2El = document.getElementById('Subject2');
                    const s1 = s1El ? s1El.value : '';
                    const s2 = s2El ? s2El.value : '';
                    if (resultEl) resultEl.textContent = Lexicon.generateParagraph(s1, s2, 4);
                });
            }

            const firstSubmit = document.querySelector('input[type="submit"]');
            if (firstSubmit) {
                firstSubmit.click();
            }
        });
    });

})();