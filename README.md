## 2. Types, chaînes et null

### 2.1 Valeur ou référence

*   **Pourquoi c1.X et s1.X n’ont-ils pas la même valeur ?**
 
    PointC est une classe donc elle est de type référence. Du coup l'affectation c2 = c1 copie l'adresse mémoire ce qui veut dire que  c1 et c2 pointent vers le même objet. 
    PointS est une structure donc de type valeur. L'affectation s2 = s1 crée une copie.  Modifier s2 n'affecte donc pas s1.

*   **Que donnerait a == b en Java avec deux String ? Pourquoi C# répond-il différemment ?**
 
    En Java a == b renverrait false car l'opérateur == compare les adresses mémoire. Alors qu'en C# l'opérateur ==  compare la valeur des chaînes, ce qui renvoie `true`.

*   **Que signifie le suffixe m ? Dans quel type de programme le choix entre double et decimal est-il important ?**
  
    Le suffixe m spécifie qu'un nombre flottant est de type decimal. Ce choix est important dans les programmes bancaires, car le type decimal fonctionne en base 10 et évite les erreurs d'arrondi.

 ### 2.2 Le null sous contrôle

 *   **Pourquoi Console.ReadLine() peut-il renvoyer null ?**
    La méthode `Console.ReadLine()` lit le flux d'entrée standard. Elle renvoie `null` lorsqu'elle atteint la fin de ce flux, ce qui signifie qu'il n'y a plus aucune donnée à lire.

*   **Que se passe-t-il si un avertissement de nullabilité réapparaît avec TreatWarningsAsErrors ?**
On demande au compilateur d'être intraitable. Si un avertissement ( apparaît, il est désormais considéré comme une erreur fatale. Du coup, la compilation échouera immédiatement et le programme refusera de se lancer tant que le code ne sera pas corrigé.

## 3. Contrôle et collections

### 4.

*  **Que se passe-t-il si vous lisez dico["Inconnu"] ?**
 le programme lève immédiatement une exception de type `KeyNotFoundException` et s'arrête (plante).

## 4. Classe Etudiant et propriétés

### 3.
### 4.2 La classe Promo

*   **Le programme compile-t-il toujours sans "public" ?**
* 
    Oui, le programme compile toujours sans problème. C'est dû au fait que  la classe `Promo` et  `Program.cs` se trouvent à l'intérieur du même projet.

*   **Quelle est la visibilité par défaut d’une classe, et dans quel cas cela poserait-il problème ?**
* 
    En C#, lorsqu'aucun modificateur d'accès n'est précisé, la visibilité par défaut d'une classe est `internal`. Cela signifie qu'elle est strictement réservée à l'assembly dans lequel elle a été déclarée. 
    Cela poserait problème dans le cas d'un projet de tests unitaires séparé. Un projet de tests est compilé comme un assembly distinct ; il serait donc incapable de voir ou d'utiliser la classe `Promo` pour la tester si celle-ci n'est pas explicitement marquée comme `public`.

## 5. Héritage et interfaces : les formes 

### 5.2 Polymorphisme

*   **Les formes s’affichent-elles par aire croissante ?**
    Oui. La méthode `Sort()` utilise l'interface `IComparable<Forme>` implémentée par la classe `Forme`. Comme notre méthode `CompareTo` a été écrite pour comparer les aires (`this.Aire().CompareTo(autre.Aire())`), le tri s'effectue bien de la plus petite à la plus grande aire.

*   **Le texte de Cercle contient-il le rayon ?**
    Oui. Bien que les objets soient stockés dans une liste générique de type `Forme`, le polymorphisme permet au programme d'exécuter la méthode `Decrire()` spécifique à la classe `Cercle` (qui inclut le rayon) grâce aux mots-clés `virtual` (dans la classe parente) et `override` (dans l'enfant).

---

### 5.3 Le piège du virtual

*   **Que dit le compilateur si on retire "virtual" de Decrire() ?**
    Le compilateur génère un avertissement indiquant que la méthode `Cercle.Decrire()` masque le membre hérité `Forme.Decrire()`. Il précise que si ce masquage est intentionnel, il faut utiliser le mot-clé `new`. Puisque nous avons activé `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, cela empêche carrément la compilation.

*   **Les deux lignes affichent-elles la même chose ?**
    Non, les deux lignes affichent des résultats différents :
    *   `c.Decrire()` (variable de type `Cercle`) affiche la description avec le rayon.
    *   `f.Decrire()` (variable de type `Forme`) affiche la description de base, sans le rayon.

*   **Différence entre new (masquage) et override (redéfinition) :**
    *   Avec **`override`** : Le polymorphisme est actif. Le programme regarde le type réel de l'objet en mémoire lors de l'exécution pour choisir la bonne méthode.
    *   Avec **`new`** : Le polymorphisme est brisé. Le programme se fie uniquement au type de la variable déclarée lors de la compilation, ignorant le type réel de l'objet contenu dedans.

*   **Comment ce code se comporterait-il en Java ?**
    En Java, le comportement serait identique à celui de l'`override` en C#. Comme toutes les méthodes d'instance sont virtuelles par défaut, il n'y a pas besoin de mot-clé spécifique pour activer le polymorphisme.

## 6. Records et exceptions : un fichier de notes

**Que vaut == entre deux objets Etudiant ayant les mêmes données ? Pourquoi ?**
    Le résultat vaut `false`. En C#, `Etudiant` est une classe. Par défaut, pour les classes, l'opérateur `==` compare les adresses mémoire des objets. Même si les deux objets `Etudiant` contiennent exactement les mêmes données, il s'agit de deux instances distinctes en mémoire, elles ont donc des adresses différentes.

