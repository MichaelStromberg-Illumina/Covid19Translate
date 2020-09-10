# Covid19Translate

In our COVID-19 project, our collaborators were providing us with lists of pseudo-HGVS p. notation and our team needed equivalent VCF files. Due to the redundancy between amino acid residues and their respective codons, it's a difficult task. However, we can always identify the true location to a particular codon location and in some cases we can identify the exact position.

We can do this because:
* we know the actual bases in the codon for the reference amino acid
* the pipeline that created the original HGVS p. notation was entirely focused on substitutions (SNVs and MNVs)

## Input

| ID | ExistingMutations                                     | 
|----|-------------------------------------------------------| 
| S1 | (NSP3_A534V)                                          | 
| S2 | (NSP2_G212D,NS3_P178L)                                | 
| S3 | 0                                                     | 
| S4 | (NSP12_P323L,Spike_D614G,N_G204R,N_R203K)             | 
| S5 | (NSP3_C1392F,NSP12_P323L,Spike_D614G,N_G204R,N_R203K) | 

## Output
Here's the tool's output:

```
- parsing GenBank file... 38 coding sequences loaded.
- parsing synonyms file... 40 synonyms loaded.
- parsing pseudo-HGVS p. nomenclature file... 4 samples loaded.

HGVS Protein Variants:
===============================
- YP_009724397.2(N):p.R203K
- YP_009724397.2(N):p.G204R
- YP_009724391.1(NS3):p.P178L
- YP_009725307.1(NSP12):p.P323L
- YP_009725298.1(NSP2):p.G212D
- YP_009725299.1(NSP3):p.A534V
- YP_009725299.1(NSP3):p.C1392F
- YP_009724390.1(Spike):p.D614G

- validating HGVS entries against gene models... finished.
- writing variants for S1... 1 SNVs written (1 exact, 0 ambiguous).
- writing variants for S2... 3 SNVs written (1 exact, 2 ambiguous).
- writing variants for S4... 8 SNVs written (3 exact, 5 ambiguous).
- writing variants for S5... 9 SNVs written (4 exact, 5 ambiguous).
```

This is what the VCF for sample S5 looks like:

```
##fileformat=VCFv4.2
##FILTER=<ID=PASS,Description="All filters passed">
##contig=<ID=NC_045512.2,length=29903>
#CHROM  POS     ID      REF     ALT     QUAL    FILTER  INFO    FORMAT  S5
NC_045512.2     6894    YP_009725299.1(NSP3):p.C1392F   G       T       .       PASS    EXACT_POSITION  GT      0/1
NC_045512.2     14407   YP_009725307.1(NSP12):p.P323L   C       T       .       PASS    AMBIGUOUS_POSITION      GT      0/1
NC_045512.2     14408   YP_009725307.1(NSP12):p.P323L   C       T       .       PASS    AMBIGUOUS_POSITION      GT      0/1
NC_045512.2     23403   YP_009724390.1(Spike):p.D614G   A       G       .       PASS    AMBIGUOUS_POSITION      GT      0/1
NC_045512.2     23404   YP_009724390.1(Spike):p.D614G   T       A       .       PASS    AMBIGUOUS_POSITION      GT      0/1
NC_045512.2     23404   YP_009724390.1(Spike):p.D614G   T       G       .       PASS    AMBIGUOUS_POSITION      GT      0/1
NC_045512.2     28881   YP_009724397.2(N):p.R203K       G       A       .       PASS    EXACT_POSITION  GT      0/1
NC_045512.2     28883   YP_009724397.2(N):p.G204R       G       C       .       PASS    EXACT_POSITION  GT      0/1
NC_045512.2     28883   YP_009724397.2(N):p.G204R       G       A       .       PASS    EXACT_POSITION  GT      0/1
```

Variants where the HGVS p. notation has been isolated to a single genomic position are marked with EXACT_POSITION. Otherwise they are marked with AMBIGUOUS_POSITION. For convenience, the original HGVS p. notation is listed in the ID field.

## Usage

```
dotnet Covid19Translate.dll NC_045512_2020-07-18.gb synonyms.tsv MutationsBySample.tsv VCF
```
